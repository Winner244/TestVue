// Test setup file
import { config } from '@vue/test-utils';
import { vi } from 'vitest';

// Mock vue-toast-notification
config.global.mocks = {
  $toast: {
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn(),
    info: vi.fn(),
  },
};

// Global test utilities can be added here
