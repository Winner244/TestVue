import { formatBooleanValue } from '../constants/booleanDisplay';

/**
 * Format ISO/UTC date string to a localized display string
 */
export const formatDate = (dateString: string): string => {
  if (!dateString) return 'N/A';
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return 'Invalid date';
  return date.toLocaleString();
};

/**
 * Convert camelCase or snake_case keys to Title Case labels
 */
export const formatFieldName = (key: string): string => {
  if (!key) return '';
  // replace underscores, then insert spaces before capitals
  const withSpaces = key
    .replace(/_/g, ' ')
    .replace(/([A-Z])/g, ' $1')
    .trim();
  return withSpaces.replace(/^./, str => str.toUpperCase());
};

/**
 * Convert field values to human-friendly strings
 */
export const formatFieldValue = (value: any): string => {
  if (value === null || value === undefined) return 'N/A';
  if (typeof value === 'boolean') return formatBooleanValue(value);
  if (Array.isArray(value)) return value.length > 0 ? value.join(', ') : 'None';
  return String(value);
};
