import { describe, it, expect } from 'vitest';
import { formatDate, formatFieldName, formatFieldValue } from '../utils/formatters';

describe('Formatters', () => {
  describe('formatDate', () => {
    it('formats valid date string', () => {
      const date = '2025-01-15T10:30:00Z';
      const formatted = formatDate(date);
      expect(formatted).toBeTruthy();
      expect(typeof formatted).toBe('string');
    });

    it('handles null date', () => {
      expect(formatDate(null as any)).toBe('N/A');
    });

    it('handles undefined date', () => {
      expect(formatDate(undefined as any)).toBe('N/A');
    });

    it('handles invalid date string', () => {
      expect(formatDate('invalid-date')).toBe('Invalid date');
    });

    it('handles empty string', () => {
      expect(formatDate('')).toBe('N/A');
    });
  });

  describe('formatFieldName', () => {
    it('converts camelCase to Title Case', () => {
      expect(formatFieldName('fullName')).toBe('Full Name');
      expect(formatFieldName('emailAddress')).toBe('Email Address');
    });

    it('converts snake_case to Title Case', () => {
      expect(formatFieldName('first_name')).toBe('First name');
      expect(formatFieldName('phone_number')).toBe('Phone number');
    });

    it('handles single word', () => {
      expect(formatFieldName('name')).toBe('Name');
    });

    it('handles multiple capital letters', () => {
      expect(formatFieldName('HTMLElement')).toBe('H T M L Element');
    });

    it('handles empty string', () => {
      expect(formatFieldName('')).toBe('');
    });
  });

  describe('formatFieldValue', () => {
    it('handles null', () => {
      expect(formatFieldValue(null)).toBe('N/A');
    });

    it('handles undefined', () => {
      expect(formatFieldValue(undefined)).toBe('N/A');
    });

    it('handles boolean true', () => {
      expect(formatFieldValue(true)).toBe('Yes');
    });

    it('handles boolean false', () => {
      expect(formatFieldValue(false)).toBe('No');
    });

    it('handles string', () => {
      expect(formatFieldValue('test string')).toBe('test string');
    });

    it('handles empty string', () => {
      expect(formatFieldValue('')).toBe('');
    });

    it('handles array', () => {
      expect(formatFieldValue(['item1', 'item2', 'item3'])).toBe('item1, item2, item3');
    });

    it('handles empty array', () => {
      expect(formatFieldValue([])).toBe('None');
    });

    it('handles number', () => {
      expect(formatFieldValue(42)).toBe('42');
    });

    it('handles object', () => {
      const obj = { key: 'value' };
      expect(formatFieldValue(obj)).toBe(String(obj));
    });
  });
});
