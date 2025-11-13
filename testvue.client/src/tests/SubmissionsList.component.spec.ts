import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import { ref } from 'vue';
import SubmissionsList from '../components/SubmissionsList/SubmissionsList.vue';

// Helper factory for mock composable state
function createMockState(options: Partial<{ loading: boolean; submissions: any[]; error: string | null }> = {}) {
  return {
    submissions: ref(options.submissions ?? []),
    loading: ref(options.loading ?? false),
    error: ref(options.error ?? null),
    loadSubmissions: vi.fn(async () => { /* no-op */ })
  };
}

// Default mock implementation (overridden per test when needed)
let currentState = createMockState();

vi.mock('../composables/useSubmissions', () => ({
  useSubmissions: () => currentState
}));

// Mock loading overlay component
vi.mock('vue-loading-overlay', () => ({
  default: {
    name: 'LoadingOverlayStub',
    template: '<div class="loading-overlay-stub" />'
  }
}));

describe('SubmissionsList.vue (component)', () => {
  beforeEach(() => {
    currentState = createMockState();
  });

  it('shows loading state when loading is true', () => {
    currentState = createMockState({ loading: true });
    const wrapper = mount(SubmissionsList);
    expect(wrapper.find('.submissions__loading').exists()).toBe(true);
    expect(wrapper.find('[aria-busy="true"]').exists()).toBe(true);
  });

  it('shows empty state when there are no submissions', () => {
    currentState = createMockState({ submissions: [], loading: false });
    const wrapper = mount(SubmissionsList);
    expect(wrapper.find('.submissions__status').text()).toMatch(/No submissions/i);
  });

  it('renders list of submissions when data available', () => {
    currentState = createMockState({ submissions: [
      { id: 'id-1', formData: { fullName: 'Alice', age: 30 }, submittedAt: '2025-01-01T10:00:00Z' },
      { id: 'id-2', formData: { fullName: 'Bob', age: 42 }, submittedAt: '2025-01-02T11:00:00Z' }
    ], loading: false });
    const wrapper = mount(SubmissionsList);
    const articles = wrapper.findAll('article.submissions__card');
    expect(articles).toHaveLength(2);
    expect(articles[0].text()).toContain('Alice');
    expect(articles[1].text()).toContain('Bob');
  });

  it('filters submissions by numeric value', async () => {
    currentState = createMockState({ submissions: [
      { id: 'id-1', formData: { fullName: 'Alice', age: 30 }, submittedAt: '2025-01-01T10:00:00Z' },
      { id: 'id-2', formData: { fullName: 'Bob', age: 42 }, submittedAt: '2025-01-02T11:00:00Z' }
    ], loading: false });
    const wrapper = mount(SubmissionsList);
    const input = wrapper.find('#search-input');
    await input.setValue('42');
    const filteredArticles = wrapper.findAll('article.submissions__card');
    expect(filteredArticles).toHaveLength(1);
    expect(filteredArticles[0].text()).toContain('Bob');
  });

  it('invokes loadSubmissions when Refresh button clicked', async () => {
    currentState = createMockState({ submissions: [], loading: false });
    const wrapper = mount(SubmissionsList);
    const btn = wrapper.find('button.submissions__button');
    await btn.trigger('click');
    expect(currentState.loadSubmissions).toHaveBeenCalledTimes(1);
  });
});
