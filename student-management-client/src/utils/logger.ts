// Comprehensive logging system with dual-tag structure as required by the assignment
// This logger tracks operations with both feature tags (user-facing functionality)
// and module tags (internal code structure) for better analysis

// TypeScript interface defining the structure of a log entry
// An interface is like a contract that ensures objects have specific properties
export interface LogEntry {
  timestamp: Date;                                    // When the log was created
  functionName: string;                               // Which function was called
  featureTag: string;                                 // User-facing feature (e.g., "StudentManagement")
  moduleTag: string;                                  // Internal module (e.g., "ApiService", "StudentListComponent")
  parameters: Record<string, any>;                    // Function parameters and their values
  level: 'info' | 'warning' | 'error' | 'debug';    // Log severity level
  message?: string;                                   // Optional descriptive message
}

// Main Logger class using the static pattern
// Static means all methods belong to the class itself, not instances
// This creates a singleton-like pattern where there's only one logger in the app
export class Logger {
  // Private static array to store all log entries in memory
  private static logs: LogEntry[] = [];
  // Maximum number of logs to keep (prevents memory overflow)
  private static maxLogs = 10000;

  // Main logging method that all other methods call
  static log(
    functionName: string,
    featureTag: string,
    moduleTag: string,
    parameters: Record<string, any>,
    level: 'info' | 'warning' | 'error' | 'debug' = 'info',  // Default to 'info' if not specified
    message?: string
  ): void {
    // Create a new log entry with all the required information
    const entry: LogEntry = {
      timestamp: new Date(),
      functionName,
      featureTag,
      moduleTag,
      parameters,
      level,
      message
    };

    // Add the entry to our logs array
    this.logs.push(entry);
    
    // Remove oldest log if we exceed the maximum (FIFO - First In, First Out)
    if (this.logs.length > this.maxLogs) {
      this.logs.shift();  // Remove the first (oldest) element
    }

    // Only output to browser console in development mode
    // import.meta.env.DEV is Vite's way to check if we're in development
    if (import.meta.env.DEV) {
      // Choose the appropriate console method based on log level
      const logMethod = level === 'error' ? console.error : 
                       level === 'warning' ? console.warn : 
                       console.log;
      
      // Format and output the log with timestamp, level, tags, and function name
      logMethod(`[${entry.timestamp.toISOString()}] [${level.toUpperCase()}] [${featureTag}] [${moduleTag}] ${functionName}`, {
        parameters,
        message
      });
    }
  }

  // Filter logs by feature tag to analyze specific user-facing functionality
  static getLogsByFeature(featureTag: string): LogEntry[] {
    return this.logs.filter(log => log.featureTag === featureTag);
  }

  // Filter logs by module tag to analyze specific code modules
  static getLogsByModule(moduleTag: string): LogEntry[] {
    return this.logs.filter(log => log.moduleTag === moduleTag);
  }

  // Get all logs (creates a copy to prevent external modification)
  static getAllLogs(): LogEntry[] {
    return [...this.logs];  // Spread operator creates a shallow copy
  }

  // Clear all logs from memory
  static clearLogs(): void {
    this.logs = [];
  }

  // Export logs as formatted JSON string for saving or analysis
  static exportLogs(): string {
    return JSON.stringify(this.logs, null, 2);  // null, 2 adds pretty formatting
  }

  // Group logs by feature tag for feature-based analysis
  // Returns a Map where keys are feature tags and values are arrays of logs
  static sortLogsByFeature(): Map<string, LogEntry[]> {
    const grouped = new Map<string, LogEntry[]>();
    
    this.logs.forEach(log => {
      // If this feature tag doesn't exist in the map, create an empty array
      if (!grouped.has(log.featureTag)) {
        grouped.set(log.featureTag, []);
      }
      // Add the log to the appropriate feature group
      // The ! tells TypeScript we know the array exists (we just created it if needed)
      grouped.get(log.featureTag)!.push(log);
    });
    
    return grouped;
  }

  // Group logs by module tag for module-based analysis
  // Returns a Map where keys are module tags and values are arrays of logs
  static sortLogsByModule(): Map<string, LogEntry[]> {
    const grouped = new Map<string, LogEntry[]>();
    
    this.logs.forEach(log => {
      // If this module tag doesn't exist in the map, create an empty array
      if (!grouped.has(log.moduleTag)) {
        grouped.set(log.moduleTag, []);
      }
      // Add the log to the appropriate module group
      grouped.get(log.moduleTag)!.push(log);
    });
    
    return grouped;
  }
}

// Factory function to create a logger instance with preset tags
// This pattern allows different parts of the app to create their own tagged loggers
// without having to specify the tags on every log call
export const createLogger = (defaultFeatureTag: string, defaultModuleTag: string) => {
  // Return an object with convenience methods for each log level
  return {
    // Info level - general information about normal operations
    info: (functionName: string, parameters: Record<string, any>, message?: string) => 
      Logger.log(functionName, defaultFeatureTag, defaultModuleTag, parameters, 'info', message),
    
    // Warning level - something unexpected but not critical
    warning: (functionName: string, parameters: Record<string, any>, message?: string) => 
      Logger.log(functionName, defaultFeatureTag, defaultModuleTag, parameters, 'warning', message),
    
    // Error level - something went wrong
    error: (functionName: string, parameters: Record<string, any>, message?: string) => 
      Logger.log(functionName, defaultFeatureTag, defaultModuleTag, parameters, 'error', message),
    
    // Debug level - detailed information for debugging
    debug: (functionName: string, parameters: Record<string, any>, message?: string) => 
      Logger.log(functionName, defaultFeatureTag, defaultModuleTag, parameters, 'debug', message)
  };
};