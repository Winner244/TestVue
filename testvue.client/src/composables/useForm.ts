import { reactive, ref } from 'vue';
import { z, ZodSchema } from 'zod';
import { useToast } from 'vue-toast-notification';

/**
 * Composable for managing form state, validation, and submission
 * @template T - The type of the form data
 * @param schema - Zod schema for validation
 * @param initialData - Initial form data
 * @param onSubmit - Callback function to handle form submission
 * @returns Form management utilities
 */
export function useForm<T extends Record<string, any>>(
    schema: ZodSchema<T>,
    initialData: T,
    onSubmit: (data: T) => Promise<void>
) {
    const $toast = useToast();
    const formData = reactive<T>({ ...initialData });
    const errors = reactive<Partial<Record<keyof T, string>>>({});
    const isSubmitting = ref(false);
    const isDirty = ref(false);

    /**
     * Clear error for a specific field
     */
    const clearFieldError = (field: keyof T) => {
        errors[field] = '';
    };

    /**
     * Validate a single field
     */
    const validateField = (field: keyof T): boolean => {
        const fieldData = { [field]: formData[field] } as Partial<T>;
        
        try {
            const fieldSchema = schema.pick({ [field]: true } as any);
            fieldSchema.parse(fieldData);
            clearFieldError(field);
            return true;
        } catch (err) {
            if (err instanceof z.ZodError && err.issues.length > 0) {
                errors[field] = err.issues[0]!.message;
            }
            return false;
        }
    };

    /**
     * Validate all form fields
     */
    const validateForm = (): boolean => {
        try {
            schema.parse(formData);
            // Clear all errors on successful validation
            Object.keys(errors).forEach(key => {
                errors[key as keyof T] = '';
            });
            return true;
        } catch (err) {
            if (err instanceof z.ZodError) {
                err.issues.forEach((error: z.ZodIssue) => {
                    const fieldName = error.path[0] as keyof T;
                    if (fieldName in errors || fieldName in formData) {
                        errors[fieldName] = error.message;
                    }
                });
            }
            return false;
        }
    };

    /**
     * Handle form submission
     */
    const handleSubmit = async () => {
        if (!validateForm()) {
            $toast.error('Please fix the validation errors', {
                position: 'top',
                duration: 3000
            });
            return;
        }

        isSubmitting.value = true;

        try {
            await onSubmit(formData);
            isDirty.value = false;
        } catch (error) {
            console.error('Error submitting form:', error);
            $toast.error('An error occurred. Please try again.', {
                position: 'top',
                duration: 5000
            });
        } finally {
            isSubmitting.value = false;
        }
    };

    /**
     * Reset form to initial state
     */
    const resetForm = () => {
        Object.assign(formData, initialData);
        Object.keys(errors).forEach(key => {
            errors[key as keyof T] = '';
        });
        isDirty.value = false;
    };

    /**
     * Mark form as dirty (modified)
     */
    const markDirty = () => {
        isDirty.value = true;
    };

    return {
        formData,
        errors,
        isSubmitting,
        isDirty,
        clearFieldError,
        validateField,
        validateForm,
        handleSubmit,
        resetForm,
        markDirty
    };
}
