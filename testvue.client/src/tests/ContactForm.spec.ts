import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import ContactForm from '../components/ContactForm/ContactForm.vue';

// Toast mocks accessible in tests
const toastSuccessMock = vi.fn();
const toastErrorMock = vi.fn();
vi.mock('vue-toast-notification', () => ({
  useToast: () => ({ success: toastSuccessMock, error: toastErrorMock })
}));

// Mock fetch
globalThis.fetch = vi.fn() as any;

describe('ContactForm', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders the form with all fields', () => {
    const wrapper = mount(ContactForm);
    
    expect(wrapper.find('#fullName').exists()).toBe(true);
    expect(wrapper.find('#email').exists()).toBe(true);
    expect(wrapper.find('#subject').exists()).toBe(true);
    expect(wrapper.find('#contactDate').exists()).toBe(true);
    expect(wrapper.find('#message').exists()).toBe(true);
    expect(wrapper.find('button[type="submit"]').exists()).toBe(true);
  });

  it('shows validation error when fullName is empty', async () => {
    const wrapper = mount(ContactForm);
    const input = wrapper.find('#fullName');
    
    await input.trigger('blur');
    await wrapper.vm.$nextTick();
    
    const errorMessage = wrapper.find('#fullName-error');
    expect(errorMessage.exists()).toBe(true);
    expect(errorMessage.text()).toContain('at least 2 characters');
  });

  it('shows validation error for invalid email', async () => {
    const wrapper = mount(ContactForm);
    const emailInput = wrapper.find('#email');
    
    await emailInput.setValue('invalid-email');
    await emailInput.trigger('blur');
    await wrapper.vm.$nextTick();
    
    const errorMessage = wrapper.find('#email-error');
    expect(errorMessage.exists()).toBe(true);
    expect(errorMessage.text()).toContain('valid email');
  });

  it('validates message length', async () => {
    const wrapper = mount(ContactForm);
    const messageInput = wrapper.find('#message');
    
    await messageInput.setValue('Short');
    await messageInput.trigger('blur');
    await wrapper.vm.$nextTick();
    
    const errorMessage = wrapper.find('#message-error');
    expect(errorMessage.exists()).toBe(true);
    expect(errorMessage.text()).toContain('at least 10 characters');
  });

  it('validates past dates', async () => {
    const wrapper = mount(ContactForm);
    const dateInput = wrapper.find('#contactDate');
    
    // Set a past date
    const pastDate = new Date();
    pastDate.setDate(pastDate.getDate() - 1);
    const pastDateString = pastDate.toISOString().split('T')[0];
    
    await dateInput.setValue(pastDateString);
    await dateInput.trigger('blur');
    await wrapper.vm.$nextTick();
    
    const errorMessage = wrapper.find('#contactDate-error');
    expect(errorMessage.exists()).toBe(true);
    expect(errorMessage.text()).toContain('cannot be in the past');
  });

  it('submits form with valid data', async () => {
    const wrapper = mount(ContactForm);
    
    // Mock successful fetch
    (globalThis.fetch as any).mockResolvedValueOnce({
      ok: true,
      json: async () => ({}),
    });
    
    // Fill form with valid data
    await wrapper.find('#fullName').setValue('John Doe');
    await wrapper.find('#email').setValue('john@example.com');
    await wrapper.find('#subject').setValue('general');
    
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    
    await wrapper.find('input[value="email"]').setValue(true);
    await wrapper.find('#message').setValue('This is a test message that is long enough');
    
    // Submit form
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    
    // Verify fetch was called
    expect(globalThis.fetch).toHaveBeenCalledWith('/api/formsubmission', expect.any(Object));
  });

  it('has proper ARIA attributes', () => {
    const wrapper = mount(ContactForm);
    
    // Check form has aria-label
    expect(wrapper.find('form').attributes('aria-label')).toBe('Contact form');
    
    // Check inputs have aria-required
    expect(wrapper.find('#fullName').attributes('aria-required')).toBe('true');
    expect(wrapper.find('#email').attributes('aria-required')).toBe('true');
  });

  it('clears errors when input becomes valid', async () => {
    const wrapper = mount(ContactForm);
    const input = wrapper.find('#fullName');
    
    // Trigger validation error
    await input.trigger('blur');
    await wrapper.vm.$nextTick();
    expect(wrapper.find('#fullName-error').exists()).toBe(true);
    
    // Fix the error
    await input.setValue('John Doe');
    await input.trigger('blur');
    await wrapper.vm.$nextTick();
    
    // Error should be cleared
    expect(wrapper.find('#fullName-error').exists()).toBe(false);
  });

  it('shows subject required error', async () => {
    const wrapper = mount(ContactForm);
    const subjectSelect = wrapper.find('#subject');
    await subjectSelect.trigger('change'); // remains empty
    await wrapper.vm.$nextTick();
    expect(wrapper.find('#subject-error').exists()).toBe(true);
    expect(wrapper.find('#subject-error').text()).toMatch(/select a subject/i);
  });

  it('shows contact method error on submit', async () => {
    const wrapper = mount(ContactForm);
    // Provide other valid fields
    await wrapper.find('#fullName').setValue('John Doe');
    await wrapper.find('#email').setValue('john@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    await wrapper.find('#message').setValue('Valid long enough message');
    // Submit without choosing contact method
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    expect(wrapper.find('#contactMethod-error').exists()).toBe(true);
    expect(wrapper.find('#contactMethod-error').text()).toMatch(/select a contact method/i);
  });

  it('includes newsletter and interests in submission payload', async () => {
    (globalThis.fetch as any).mockResolvedValueOnce({ ok: true, json: async () => ({}) });
    const wrapper = mount(ContactForm);
    await wrapper.find('#fullName').setValue('John Doe');
    await wrapper.find('#email').setValue('john@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    // Select interests
    const productsCheckbox = wrapper.find('input[type="checkbox"][value="products"]');
    const servicesCheckbox = wrapper.find('input[type="checkbox"][value="services"]');
    await productsCheckbox.setValue(true);
    await servicesCheckbox.setValue(true);
    // Newsletter
    const newsletterCheckbox = wrapper.find('input[type="checkbox"]:not([value])');
    await newsletterCheckbox.setValue(true);
    // Contact method
    await wrapper.find('input[type="radio"][value="email"]').setValue(true);
    await wrapper.find('#message').setValue('Message content long enough');
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    const fetchCall = (globalThis.fetch as any).mock.calls[0];
    const body = JSON.parse(fetchCall[1].body);
    expect(body.interests).toEqual(['products', 'services']);
    expect(body.newsletter).toBe(true);
    expect(body.contactMethod).toBe('email');
  });

  it('resets form after successful submit', async () => {
    (globalThis.fetch as any).mockResolvedValueOnce({ ok: true, json: async () => ({}) });
    const wrapper = mount(ContactForm);
    await wrapper.find('#fullName').setValue('Jane Roe');
    await wrapper.find('#email').setValue('jane@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    await wrapper.find('input[type="radio"][value="phone"]').setValue(true);
    await wrapper.find('#message').setValue('Some message content long enough');
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    expect((wrapper.find('#fullName').element as HTMLInputElement).value).toBe('');
    expect((wrapper.find('#email').element as HTMLInputElement).value).toBe('');
    expect((wrapper.find('#subject').element as HTMLSelectElement).value).toBe('');
    expect((wrapper.find('#message').element as HTMLTextAreaElement).value).toBe('');
  });

  it('disables submit button while submitting', async () => {
    // Create a pending fetch promise to inspect disabled state
    let resolveFetch: any;
    const pending = new Promise(r => { resolveFetch = r; });
    (globalThis.fetch as any).mockImplementationOnce(() => pending);
    const wrapper = mount(ContactForm);
    await wrapper.find('#fullName').setValue('Jane Roe');
    await wrapper.find('#email').setValue('jane@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    await wrapper.find('input[type="radio"][value="email"]').setValue(true);
    await wrapper.find('#message').setValue('A sufficiently long message body');
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    const button = wrapper.find('button[type="submit"]');
    expect(button.attributes('disabled')).toBeDefined();
    // Resolve fetch and await next tick to re-enable
    resolveFetch({ ok: true, json: async () => ({}) });
    await wrapper.vm.$nextTick();
  });

  it('shows success toast on successful submit', async () => {
    (globalThis.fetch as any).mockResolvedValueOnce({ ok: true, json: async () => ({}) });
    const wrapper = mount(ContactForm);
    await wrapper.find('#fullName').setValue('Toast User');
    await wrapper.find('#email').setValue('toast@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    await wrapper.find('input[type="radio"][value="email"]').setValue(true);
    await wrapper.find('#message').setValue('Toast message long enough');
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    expect(toastSuccessMock).toHaveBeenCalled();
  });

  it('shows error toast on failed submit', async () => {
    (globalThis.fetch as any).mockResolvedValueOnce({ ok: false, json: async () => ({}) });
    const wrapper = mount(ContactForm);
    await wrapper.find('#fullName').setValue('Err User');
    await wrapper.find('#email').setValue('err@example.com');
    await wrapper.find('#subject').setValue('general');
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 1);
    await wrapper.find('#contactDate').setValue(futureDate.toISOString().split('T')[0]);
    await wrapper.find('input[type="radio"][value="email"]').setValue(true);
    await wrapper.find('#message').setValue('Error message long enough');
    await wrapper.find('form').trigger('submit.prevent');
    await wrapper.vm.$nextTick();
    expect(toastErrorMock).toHaveBeenCalled();
  });
});
