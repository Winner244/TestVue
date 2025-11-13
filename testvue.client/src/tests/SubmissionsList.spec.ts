import { describe, it, expect, beforeEach } from 'vitest';
import { computed, ref } from 'vue';
import { formatBooleanValue } from '../constants/booleanDisplay';

describe('SubmissionsList filtering', () => {
  const mockSubmissions = ref([
    {
      id: 'abc123',
      formData: {
        fullName: 'John Doe',
        email: 'john@example.com',
        subject: 'general',
        message: 'Hello world',
        newsletter: true,
      },
      submittedAt: '2025-01-01T10:00:00Z',
    },
    {
      id: 'def456',
      formData: {
        fullName: 'Jane Smith',
        email: 'jane@example.com',
        subject: 'support',
        message: 'Need help',
        newsletter: false,
      },
      submittedAt: '2025-01-02T11:00:00Z',
    },
    {
      id: 'ghi789',
      formData: {
        fullName: 'Bob Johnson',
        email: 'bob@example.com',
        subject: 'sales',
        message: 'Interested in products',
        newsletter: true,
        interests: ['products', 'services'],
      },
      submittedAt: '2025-01-03T12:00:00Z',
    },
  ]);

  // Simulate the filtering logic from SubmissionsList
  const createFilteredSubmissions = (searchQuery: string) => {
    return computed(() => {
      if (!searchQuery.trim()) {
        return mockSubmissions.value;
      }

      const query = searchQuery.trim().toLowerCase();
      return mockSubmissions.value.filter((submission) => {
        // Search in ID
        if (submission.id.toLowerCase().includes(query)) {
          return true;
        }

        // Search in form data values
        return Object.values(submission.formData).some((value) => {
          if (typeof value === 'string') {
            return value.toLowerCase().includes(query);
          }

          if (typeof value === 'boolean') {
            return formatBooleanValue(value).toLowerCase().includes(query);
          }

          if (Array.isArray(value)) {
            return value.some(
              (item) => typeof item === 'string' && item.toLowerCase().includes(query)
            );
          }
          return false;
        });
      });
    });
  };

  it('returns all submissions when search query is empty', () => {
    const filtered = createFilteredSubmissions('');
    expect(filtered.value).toHaveLength(3);
  });

  it('filters by submission ID', () => {
    const filtered = createFilteredSubmissions('abc');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.id).toBe('abc123');
  });

  it('filters by name', () => {
    const filtered = createFilteredSubmissions('jane');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.fullName).toBe('Jane Smith');
  });

  it('filters by email', () => {
    const filtered = createFilteredSubmissions('bob@example');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.email).toBe('bob@example.com');
  });

  it('filters by subject', () => {
    const filtered = createFilteredSubmissions('support');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.subject).toBe('support');
  });

  it('filters by message content', () => {
    const filtered = createFilteredSubmissions('hello');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.message).toBe('Hello world');
  });

  it('filters by boolean value (Yes)', () => {
    const filtered = createFilteredSubmissions('yes');
    expect(filtered.value).toHaveLength(2); // Two submissions have newsletter: true
  });

  it('filters by boolean value (No)', () => {
    const filtered = createFilteredSubmissions('no');
    expect(filtered.value).toHaveLength(1); // One submission has newsletter: false
  });

  it('filters by array values', () => {
    const filtered = createFilteredSubmissions('products');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.interests).toContain('products');
  });

  it('is case insensitive', () => {
    const filtered = createFilteredSubmissions('JANE');
    expect(filtered.value).toHaveLength(1);
    expect(filtered.value[0]!.formData.fullName).toBe('Jane Smith');
  });

  it('returns empty array when no matches found', () => {
    const filtered = createFilteredSubmissions('nonexistent');
    expect(filtered.value).toHaveLength(0);
  });

  it('handles multiple word search', () => {
    const filtered = createFilteredSubmissions('jane smith');
    expect(filtered.value).toHaveLength(1);
  });
});
