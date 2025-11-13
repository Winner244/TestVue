/**
 * Environment configuration
 * Access environment variables with type safety
 */

export const env = {
  /**
   * Base URL for API requests
   * @default '/api'
   */
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL || '/api',

  /**
   * Check if running in development mode
   */
  isDevelopment: import.meta.env.DEV,

  /**
   * Check if running in production mode
   */
  isProduction: import.meta.env.PROD,
} as const;
