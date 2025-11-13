<template>
    <div class="contact-form">
        <h2 class="contact-form__heading">Contact Form</h2>
        <form @submit.prevent="handleSubmit" class="contact-form__card" aria-label="Contact form" novalidate>

            <!-- Text Field: Full Name -->
            <div class="contact-form__group">
                <label for="fullName" class="contact-form__label">Full Name *</label>
                <input 
                    type="text" 
                    id="fullName" 
                    v-model="formData.fullName" 
                    @blur="validateField('fullName')"
                    class="contact-form__input" 
                    :class="{ 'contact-form__input--error': errors.fullName }"
                    :aria-invalid="!!errors.fullName"
                    :aria-describedby="errors.fullName ? 'fullName-error' : undefined"
                    aria-required="true"
                    autocomplete="name" />
                <span v-if="errors.fullName" id="fullName-error" class="contact-form__error-message" role="alert">{{ errors.fullName }}</span>
            </div>

            <!-- Email Field -->
            <div class="contact-form__group">
                <label for="email" class="contact-form__label">Email Address *</label>
                <input 
                    type="email" 
                    id="email" 
                    v-model="formData.email" 
                    @blur="validateField('email')"
                    class="contact-form__input" 
                    :class="{ 'contact-form__input--error': errors.email }"
                    :aria-invalid="!!errors.email"
                    :aria-describedby="errors.email ? 'email-error' : undefined"
                    aria-required="true"
                    autocomplete="email" />
                <span v-if="errors.email" id="email-error" class="contact-form__error-message" role="alert">{{ errors.email }}</span>
            </div>

            <!-- Dropdown: Subject -->
            <div class="contact-form__group">
                <label for="subject" class="contact-form__label">Subject *</label>
                <select 
                    id="subject" 
                    v-model="formData.subject" 
                    @change="validateField('subject')"
                    class="contact-form__select" 
                    :class="{ 'contact-form__select--error': errors.subject }"
                    :aria-invalid="!!errors.subject"
                    :aria-describedby="errors.subject ? 'subject-error' : undefined"
                    aria-required="true">
                    <option value="">Select a subject</option>
                    <option value="general">General Inquiry</option>
                    <option value="support">Technical Support</option>
                    <option value="sales">Sales</option>
                    <option value="feedback">Feedback</option>
                </select>
                <span v-if="errors.subject" id="subject-error" class="contact-form__error-message" role="alert">{{ errors.subject }}</span>
            </div>

            <!-- Date Field: Preferred Contact Date -->
            <div class="contact-form__group">
                <label for="contactDate" class="contact-form__label">Preferred Contact Date *</label>
                <input 
                    type="date" 
                    id="contactDate" 
                    v-model="formData.contactDate" 
                    @blur="validateField('contactDate')"
                    class="contact-form__input" 
                    :class="{ 'contact-form__input--error': errors.contactDate }"
                    :aria-invalid="!!errors.contactDate"
                    :aria-describedby="errors.contactDate ? 'contactDate-error' : undefined"
                    aria-required="true" />
                <span v-if="errors.contactDate" id="contactDate-error" class="contact-form__error-message" role="alert">{{ errors.contactDate }}</span>
            </div>

            <!-- Radio Buttons: Contact Method -->
            <div class="contact-form__group">
                <fieldset class="contact-form__fieldset">
                    <legend class="contact-form__label">Preferred Contact Method *</legend>
                    <div class="contact-form__radio-group" role="radiogroup" :aria-invalid="!!errors.contactMethod" :aria-describedby="errors.contactMethod ? 'contactMethod-error' : undefined">
                        <label class="contact-form__radio-label">
                            <input type="radio" name="contactMethod" value="email" v-model="formData.contactMethod"
                                @change="validateField('contactMethod')" aria-required="true" />
                            Email
                        </label>
                        <label class="contact-form__radio-label">
                            <input type="radio" name="contactMethod" value="phone" v-model="formData.contactMethod"
                                @change="validateField('contactMethod')" aria-required="true" />
                            Phone
                        </label>
                        <label class="contact-form__radio-label">
                            <input type="radio" name="contactMethod" value="sms" v-model="formData.contactMethod"
                                @change="validateField('contactMethod')" aria-required="true" />
                            SMS
                        </label>
                    </div>
                    <span v-if="errors.contactMethod" id="contactMethod-error" class="contact-form__error-message" role="alert">{{ errors.contactMethod }}</span>
                </fieldset>
            </div>

            <!-- Checkboxes: Interests -->
            <div class="contact-form__group">
                <fieldset class="contact-form__fieldset">
                    <legend class="contact-form__label">Areas of Interest</legend>
                    <div class="contact-form__checkbox-group" role="group" aria-label="Select your areas of interest">
                        <label class="contact-form__checkbox-label">
                            <input type="checkbox" value="products" v-model="formData.interests" />
                            Products
                        </label>
                        <label class="contact-form__checkbox-label">
                            <input type="checkbox" value="services" v-model="formData.interests" />
                            Services
                        </label>
                        <label class="contact-form__checkbox-label">
                            <input type="checkbox" value="partnership" v-model="formData.interests" />
                            Partnership
                        </label>
                    </div>
                </fieldset>
            </div>

            <!-- Text Area: Message -->
            <div class="contact-form__group">
                <label for="message" class="contact-form__label">Message *</label>
                <textarea 
                    id="message" 
                    v-model="formData.message" 
                    rows="5" 
                    @blur="validateField('message')"
                    class="contact-form__textarea"
                    :class="{ 'contact-form__textarea--error': errors.message }"
                    :aria-invalid="!!errors.message"
                    :aria-describedby="errors.message ? 'message-error' : undefined"
                    aria-required="true"></textarea>
                <span v-if="errors.message" id="message-error" class="contact-form__error-message" role="alert">{{ errors.message }}</span>
            </div>

            <!-- Checkbox: Newsletter -->
            <div class="contact-form__group">
                <label class="contact-form__checkbox-label">
                    <input type="checkbox" v-model="formData.newsletter" />
                    Subscribe to newsletter
                </label>
            </div>

            <!-- Submit Button -->
            <div class="contact-form__actions">
                <button 
                    type="submit" 
                    class="contact-form__button" 
                    :disabled="isSubmitting"
                    :aria-busy="isSubmitting">
                    {{ isSubmitting ? 'Submitting...' : 'Submit' }}
                </button>
            </div>
        </form>
    </div>
</template>

<style scoped lang="less" src="./contact-form.less"></style>

<script setup lang="ts">
    import { z } from 'zod';
    import { useToast } from 'vue-toast-notification';
    import { useForm } from '../../composables/useForm';

    // Zod schema for contact form validation
    const contactFormSchema = z.object({
        fullName: z.string()
            .min(2, 'Full name must be at least 2 characters')
            .min(1, 'Full name is required'),
        email: z.string()
            .min(1, 'Email is required')
            .email('Please enter a valid email address'),
        subject: z.string()
            .min(1, 'Please select a subject'),
        contactDate: z.string()
            .min(1, 'Please select a date')
            .refine((date) => {
                const selectedDate = new Date(date);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                return selectedDate >= today;
            }, 'Date cannot be in the past'),
        contactMethod: z.string()
            .min(1, 'Please select a contact method'),
        interests: z.array(z.string()).optional(),
        message: z.string()
            .min(1, 'Message is required')
            .min(10, 'Message must be at least 10 characters'),
        newsletter: z.boolean().optional(),
        formType: z.string().optional()
    });

    type ContactFormData = z.infer<typeof contactFormSchema>;

    const $toast = useToast();

    // Initial form data
    const initialData: ContactFormData = {
        fullName: '',
        email: '',
        subject: '',
        contactDate: '',
        contactMethod: '',
        interests: [],
        message: '',
        newsletter: false,
        formType: 'contact'
    };

    // Submit handler
    const submitForm = async (data: ContactFormData) => {
        const response = await fetch('/api/formsubmission', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (response.ok) {
            $toast.success('Form submitted successfully!', {
                position: 'top',
                duration: 5000
            });
            resetForm();
        } else {
            throw new Error('Failed to submit form');
        }
    };

    // Use the form composable
    const {
        formData,
        errors,
        isSubmitting,
        validateField,
        handleSubmit,
        resetForm
    } = useForm(contactFormSchema, initialData, submitForm);
</script>
