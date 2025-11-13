/**
 * Constants for boolean value display
 */
export const BOOLEAN_DISPLAY = {
  TRUE: 'Yes',
  FALSE: 'No'
} as const;

/**
 * Format boolean value to display string
 */
export const formatBooleanValue = (value: boolean): string => {
  return value ? BOOLEAN_DISPLAY.TRUE : BOOLEAN_DISPLAY.FALSE;
};
