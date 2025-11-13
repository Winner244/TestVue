<template>
    <div class="form-container">
        <h2>Contact Form</h2>
        <form @submit.prevent="handleSubmit" class="contact-form">
            <!-- Text Field: Full Name -->
            <div class="form-group">
                <label for="fullName">Full Name *</label>
                <input type="text" id="fullName" v-model="formData.fullName" @blur="validateField('fullName')"
                    :class="{ 'error': errors.fullName }" />
                <span v-if="errors.fullName" class="error-message">{{ errors.fullName }}</span>
            </div>

            <!-- Email Field -->
            <div class="form-group">
                <label for="email">Email Address *</label>
                <input type="email" id="email" v-model="formData.email" @blur="validateField('email')"
                    :class="{ 'error': errors.email }" />
                <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
            </div>

            <!-- Dropdown: Subject -->
            <div class="form-group">
                <label for="subject">Subject *</label>
                <select id="subject" v-model="formData.subject" @change="validateField('subject')"
                    :class="{ 'error': errors.subject }">
                    <option value="">Select a subject</option>
                    <option value="general">General Inquiry</option>
                    <option value="support">Technical Support</option>
                    <option value="sales">Sales</option>
                    <option value="feedback">Feedback</option>
                </select>
                <span v-if="errors.subject" class="error-message">{{ errors.subject }}</span>
            </div>

            <!-- Date Field: Preferred Contact Date -->
            <div class="form-group">
                <label for="contactDate">Preferred Contact Date *</label>
                <input type="date" id="contactDate" v-model="formData.contactDate" @blur="validateField('contactDate')"
                    :class="{ 'error': errors.contactDate }" />
                <span v-if="errors.contactDate" class="error-message">{{ errors.contactDate }}</span>
            </div>

            <!-- Radio Buttons: Contact Method -->
            <div class="form-group">
                <label>Preferred Contact Method *</label>
                <div class="radio-group">
                    <label class="radio-label">
                        <input type="radio" name="contactMethod" value="email" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        Email
                    </label>
                    <label class="radio-label">
                        <input type="radio" name="contactMethod" value="phone" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        Phone
                    </label>
                    <label class="radio-label">
                        <input type="radio" name="contactMethod" value="sms" v-model="formData.contactMethod"
                            @change="validateField('contactMethod')" />
                        SMS
                    </label>
                </div>
                <span v-if="errors.contactMethod" class="error-message">{{ errors.contactMethod }}</span>
            </div>

            <!-- Checkboxes: Interests -->
            <div class="form-group">
                <label>Areas of Interest</label>
                <div class="checkbox-group">
                    <label class="checkbox-label">
                        <input type="checkbox" value="products" v-model="formData.interests" />
                        Products
                    </label>
                    <label class="checkbox-label">
                        <input type="checkbox" value="services" v-model="formData.interests" />
                        Services
                    </label>
                    <label class="checkbox-label">
                        <input type="checkbox" value="partnership" v-model="formData.interests" />
                        Partnership
                    </label>
                </div>
            </div>

            <!-- Text Area: Message -->
            <div class="form-group">
                <label for="message">Message *</label>
                <textarea id="message" v-model="formData.message" rows="5" @blur="validateField('message')"
                    :class="{ 'error': errors.message }"></textarea>
                <span v-if="errors.message" class="error-message">{{ errors.message }}</span>
            </div>

            <!-- Checkbox: Newsletter -->
            <div class="form-group">
                <label class="checkbox-label">
                    <input type="checkbox" v-model="formData.newsletter" />
                    Subscribe to newsletter
                </label>
            </div>

            <!-- Submit Button -->
            <div class="form-actions">
                <button type="submit" class="btn-submit" :disabled="isSubmitting">
                    {{ isSubmitting ? 'Submitting...' : 'Submit' }}
                </button>
            </div>

            <!-- Success Message -->
            <div v-if="successMessage" class="success-message">
                {{ successMessage }}
            </div>
        </form>
    </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';

const formData = reactive({
    fullName: '',
    email: '',
    subject: '',
    contactDate: '',
    contactMethod: '',
    interests: [] as string[],
    message: '',
    newsletter: false,
    formType: 'contact' // To identify this form type in backend
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

const validateField = (field: string) => {
    switch (field) {
        case 'fullName':
            if (!formData.fullName.trim()) {
                errors.fullName = 'Full name is required';
            } else if (formData.fullName.trim().length < 2) {
                errors.fullName = 'Full name must be at least 2 characters';
            } else {
                errors.fullName = '';
            }
            break;

        case 'email':
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!formData.email.trim()) {
                errors.email = 'Email is required';
            } else if (!emailRegex.test(formData.email)) {
                errors.email = 'Please enter a valid email address';
            } else {
                errors.email = '';
            }
            break;

        case 'subject':
            if (!formData.subject) {
                errors.subject = 'Please select a subject';
            } else {
                errors.subject = '';
            }
            break;

        case 'contactDate':
            if (!formData.contactDate) {
                errors.contactDate = 'Please select a date';
            } else {
                const selectedDate = new Date(formData.contactDate);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                if (selectedDate < today) {
                    errors.contactDate = 'Date cannot be in the past';
                } else {
                    errors.contactDate = '';
                }
            }
            break;

        case 'contactMethod':
            if (!formData.contactMethod) {
                errors.contactMethod = 'Please select a contact method';
            } else {
                errors.contactMethod = '';
            }
            break;

        case 'message':
            if (!formData.message.trim()) {
                errors.message = 'Message is required';
            } else if (formData.message.trim().length < 10) {
                errors.message = 'Message must be at least 10 characters';
            } else {
                errors.message = '';
            }
            break;
    }
};

const validateForm = (): boolean => {
    validateField('fullName');
    validateField('email');
    validateField('subject');
    validateField('contactDate');
    validateField('contactMethod');
    validateField('message');

    return !Object.values(errors).some(error => error !== '');
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

<style scoped>
.form-container {
    max-width: 600px;
    margin: 0 auto;
    padding: 2rem;
}

h2 {
    color: #2c3e50;
    margin-bottom: 1.5rem;
    text-align: center;
}

.contact-form {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-group {
    margin-bottom: 1.5rem;
}

label {
    display: block;
    margin-bottom: 0.5rem;
    color: #2c3e50;
    font-weight: 500;
}

input[type="text"],
input[type="email"],
input[type="date"],
select,
textarea {
    width: 100%;
    padding: 0.75rem;
    border: 2px solid #e0e0e0;
    border-radius: 4px;
    font-size: 1rem;
    transition: border-color 0.3s;
    box-sizing: border-box;
}

input[type="text"]:focus,
input[type="email"]:focus,
input[type="date"]:focus,
select:focus,
textarea:focus {
    outline: none;
    border-color: #42b983;
}

input.error,
select.error,
textarea.error {
    border-color: #e74c3c;
}

.error-message {
    display: block;
    color: #e74c3c;
    font-size: 0.875rem;
    margin-top: 0.25rem;
}

.radio-group,
.checkbox-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-top: 0.5rem;
}

.radio-label,
.checkbox-label {
    display: flex;
    align-items: center;
    font-weight: normal;
    cursor: pointer;
}

.radio-label input,
.checkbox-label input {
    margin-right: 0.5rem;
    width: auto;
    cursor: pointer;
}

.form-actions {
    margin-top: 2rem;
}

.btn-submit {
    width: 100%;
    padding: 1rem;
    background-color: #42b983;
    color: white;
    border: none;
    border-radius: 4px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: background-color 0.3s;
}

.btn-submit:hover:not(:disabled) {
    background-color: #359268;
}

.btn-submit:disabled {
    background-color: #a0a0a0;
    cursor: not-allowed;
}

.success-message {
    margin-top: 1rem;
    padding: 1rem;
    background-color: #d4edda;
    color: #155724;
    border: 1px solid #c3e6cb;
    border-radius: 4px;
    text-align: center;
}
</style>
