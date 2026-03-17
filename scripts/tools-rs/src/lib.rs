//! Biblioteca del contrato de herramientas GesFer (SddIA/tools/tools-contract.json).
//! Tipos y helpers para emitir resultado JSON y feedback.

use chrono::Utc;
use serde::{Deserialize, Serialize};

/// Nivel de una entrada de feedback.
#[derive(Debug, Clone, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "lowercase")]
pub enum FeedbackLevel {
    Info,
    Warning,
    Error,
}

/// Una entrada de feedback (fase, nivel, mensaje, timestamp).
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct FeedbackEntry {
    pub phase: String,
    #[serde(rename = "level")]
    pub level: FeedbackLevel,
    pub message: String,
    pub timestamp: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub detail: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none", rename = "duration_ms")]
    pub duration_ms: Option<u64>,
}

impl FeedbackEntry {
    pub fn info(phase: &str, message: &str) -> Self {
        Self {
            phase: phase.to_string(),
            level: FeedbackLevel::Info,
            message: message.to_string(),
            timestamp: Utc::now().to_rfc3339(),
            detail: None,
            duration_ms: None,
        }
    }
    pub fn warning(phase: &str, message: &str, detail: Option<&str>) -> Self {
        Self {
            phase: phase.to_string(),
            level: FeedbackLevel::Warning,
            message: message.to_string(),
            timestamp: Utc::now().to_rfc3339(),
            detail: detail.map(String::from),
            duration_ms: None,
        }
    }
    pub fn error(phase: &str, message: &str, detail: Option<&str>) -> Self {
        Self {
            phase: phase.to_string(),
            level: FeedbackLevel::Error,
            message: message.to_string(),
            timestamp: Utc::now().to_rfc3339(),
            detail: detail.map(String::from),
            duration_ms: None,
        }
    }
}

/// Resultado de una herramienta según el contrato (toolId, exitCode, success, timestamp, message, feedback, data, duration_ms).
#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ToolResult {
    pub tool_id: String,
    pub exit_code: i32,
    pub success: bool,
    pub timestamp: String,
    pub message: String,
    pub feedback: Vec<FeedbackEntry>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub data: Option<serde_json::Value>,
    #[serde(skip_serializing_if = "Option::is_none", rename = "duration_ms")]
    pub duration_ms: Option<u64>,
}

impl ToolResult {
    #[allow(dead_code)]
    pub fn ok(tool_id: &str, message: &str, feedback: Vec<FeedbackEntry>, data: Option<serde_json::Value>, duration_ms: u64) -> Self {
        Self {
            tool_id: tool_id.to_string(),
            exit_code: 0,
            success: true,
            timestamp: Utc::now().to_rfc3339(),
            message: message.to_string(),
            feedback,
            data,
            duration_ms: Some(duration_ms),
        }
    }
    pub fn err(tool_id: &str, message: &str, feedback: Vec<FeedbackEntry>, exit_code: i32, data: Option<serde_json::Value>, duration_ms: u64) -> Self {
        Self {
            tool_id: tool_id.to_string(),
            exit_code,
            success: false,
            timestamp: Utc::now().to_rfc3339(),
            message: message.to_string(),
            feedback,
            data,
            duration_ms: Some(duration_ms),
        }
    }
}

/// Serializa el resultado a JSON según el contrato (toolId, exitCode, success, timestamp, message, feedback, data, duration_ms).
pub fn to_contract_json(result: &ToolResult) -> Result<String, serde_json::Error> {
    serde_json::to_string(result)
}
