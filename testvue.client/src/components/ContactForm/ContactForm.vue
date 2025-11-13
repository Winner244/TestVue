<template>
    <div class="contact-form">
        <h2 class="contact-form__heading">Contact Form</h2>
        <form @submit.prevent="handleSubmit" class="contact-form__card">
            <!-- Success Message -->
            <div v-if="successMessage" class="contact-form__success-message">
                {{ successMessage }}
            </div>

            <!-- Text Field: Full Name -->
            <div class="contact-form__group">
                <label for="fullName" class="contact-form__label">Full Name *</label>
                <input type="text" id="fullName" v-model="formData.fullName" @blur="validateField('fullName')"
                    class="contact-form__input" :class="{ 'contact-form__input--error': errors.fullName }" />
                <span v-if="errors.fullName" class="contact-form__error-message">{{ errors.fullName }}</span>
            </div>

            <!-- Email Field -->
            <div class="contact-form__group">
                <label for="email" class="contact-form__label">Email Address *</label>
                <input type="email" id="email" v-model="formData.email" @blur="validateField('email')"
                    class="contact-form__input" :class="{ 'contact-form__input--error': errors.email }" />
                <span v-if="errors.email" class="contact-form__error-message">{{ errors.email }}</span>
            </div>

            <!-- Dropdown: Subject -->
            <div class="contact-form__group">
                <label for="subject" class="contact-form__label">Subject *</label>
                <select id="subject" v-model="formData.subject" @change="validateField('subject')"
                    class="contact-form__select" :class="{ 'contact-form__select--error': errors.subject }">
                    <option value="">Select a subject</option>
                    <option value="general">General Inquiry</option>
                    <option value="support">Technical Support</option>
                    <option value="sales">Sales</option>
                    <option value="feedback">Feedback</option>
                </select>
                <span v-if="errors.subject" class="contact-form__error-message">{{ errors.subject }}</span>
            </div>

            <!-- Date Field: Preferred Contact Date -->
            <div class="contact-form__group">
                <label for="contactDate" class="contact-form__label">Preferred Contact Date *</label>
                <input type="date" id="contactDate" v-model="formData.contactDate" @blur="validateField('contactDate')"
                    class="contact-form__input" :class="{ 'contact-form__input--error': errors.contactDate }" />
                <span v-if="errors.contactDate" class="contact-form__error-message">{{ errors.contactDate }}</span>
            </div>

            <!-- Radio Buttons: Contact Method -->
            <div class="contact-form__group">
                <label class="contact-form__label">Preferred Contact Method *</label>
                <div class="contact-form__radio-group">
                    <label class="contact-form__radio-label">
                        <input type="radio" name="contactMethod" value="email" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        Email
                    </label>
                    <label class="contact-form__radio-label">
                        <input type="radio" name="contactMethod" value="phone" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        Phone
                    </label>
                    <label class="contact-form__radio-label">
                        <input type="radio" name="contactMethod" value="sms" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        SMS
                    </label>
                </div>
                <span v-if="errors.contactMethod" class="contact-form__error-message">{{ errors.contactMethod }}</span>
            </div>

            <!-- Checkboxes: Interests -->
            <div class="contact-form__group">
                <label class="contact-form__label">Areas of Interest</label>
                <div class="contact-form__checkbox-group">
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
            </div>

            <!-- Text Area: Message -->
            <div class="contact-form__group">
                <label for="message" class="contact-form__label">Message *</label>
                <textarea id="message" v-model="formData.message" rows="5" @blur="validateField('message')"
                    class="contact-form__textarea"
                    :class="{ 'contact-form__textarea--error': errors.message }"></textarea>
                <span v-if="errors.message" class="contact-form__error-message">{{ errors.message }}</span>
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
                <button type="submit" class="contact-form__button" :disabled="isSubmitting">
                    {{ isSubmitting ? 'Submitting...' : 'Submit' }}
                </button>
            </div>
        </form>
    </div>
</template>

<style scoped lang="less" src="./contact-form.less"></style>

<script setup lang="ts">
    import { reactive, ref } from 'vue';
    import { z } from 'zod';

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

    const formData = reactive({
        fullName: '',
        email: '',
        subject: '',
        contactDate: '',
        contactMethod: '',
        interests: [] as string[],
        message: '',
        newsletter: false,
        formType: 'contact'
    });

    const errors = reactive({
        fullName: '',
        email: '',
        subject: '',
        contactDate: '',
        contactMethod: '',
        message: ''
    });

    const isSubmitting = ref(false);
    const successMessage = ref('');

    const clearFieldError = (field: keyof typeof errors) => {
        errors[field] = '';
    };

    const validateField = (field: string) => {
        const fieldData = { [field]: formData[field as keyof typeof formData] };
        
        try {
            const schema = contactFormSchema.pick({
                [field]: true
            } as any);
            schema.parse(fieldData);
            clearFieldError(field as keyof typeof errors);
        } catch (err) {
            if (err instanceof z.ZodError && err.issues.length > 0) {
                errors[field as keyof typeof errors] = err.issues[0]!.message;
            }
        }
    };

    const validateForm = (): boolean => {
        try {
            contactFormSchema.parse(formData);
            // Clear all errors on successful validation
            Object.keys(errors).forEach(key => {
                errors[key as keyof typeof errors] = '';
            });
            return true;
        } catch (err) {
            if (err instanceof z.ZodError) {
                err.issues.forEach((error: z.ZodIssue) => {
                    const fieldName = error.path[0] as keyof typeof errors;
                    if (fieldName in errors) {
                        errors[fieldName] = error.message;
                    }
                });
            }
            return false;
        }
    };

    const handleSubmit = async () => {
        if (!validateForm()) {
            return;
        }

        isSubmitting.value = true;
        successMessage.value = '';

        try {
            const response = await fetch('/api/formsubmission', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });

            if (response.ok) {
                successMessage.value = 'Form submitted successfully!';
                // Reset form
                Object.assign(formData, {
                    fullName: '',
                    email: '',
                    subject: '',
                    contactDate: '',
                    contactMethod: '',
                    interests: [],
                    message: '',
                    newsletter: false,
                    formType: 'contact'
                });
            } else {
                alert('Failed to submit form. Please try again.');
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            alert('An error occurred. Please try again.');
        } finally {
            isSubmitting.value = false;
        }
    };
</script>
