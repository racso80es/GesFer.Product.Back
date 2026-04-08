import os
import re

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Patrones para buscar y reemplazar:
    # 1. `.Where(x => x.DeletedAt == null)` o similares enteros -> remover
    # 2. `&& x.DeletedAt == null` -> remover
    # 3. `x.DeletedAt == null && ` -> remover

    original = content

    # && x.DeletedAt == null
    content = re.sub(r'\s*&&\s*\w+\.DeletedAt\s*==\s*null', '', content)
    # x.DeletedAt == null &&
    content = re.sub(r'\w+\.DeletedAt\s*==\s*null\s*&&\s*', '', content)

    # .Where(x => x.DeletedAt == null) -> a veces deja .Where() vacio
    # .Where(x => ) -> hay que removerlo o dejar .AsQueryable()
    content = re.sub(r'\.Where\(\s*\w+\s*=>\s*\w+\.DeletedAt\s*==\s*null\s*\)', '.AsQueryable()', content)

    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"Modificado: {filepath}")

for root, dirs, files in os.walk('src/'):
    for file in files:
        if file.endswith('.cs'):
            process_file(os.path.join(root, file))
