import { reactive, ref, toRaw } from 'vue';
import { z, type ZodSchema } from 'zod';
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
    const errors = reactive<Record<string, string>>({});
    const isSubmitting = ref(false);

    /**
     * Clear error for a specific field
     */
    const clearFieldError = (field: string) => {
        (errors as any)[field] = '';
    };

    /**
     * Validate a single field
     */
    const validateField = (field: string): boolean => {
        const fieldData = { [field]: (formData as any)[field] } as Partial<T>;
        
        try {
            const fieldSchema = (schema as any).pick({ [field]: true });
            fieldSchema.parse(fieldData);
            clearFieldError(field);
            return true;
        } catch (err) {
            if (err instanceof z.ZodError && err.issues.length > 0) {
                (errors as any)[field] = err.issues[0]!.message;
            }
            return false;
        }
    };

    /**
     * Validate all form fields
     */
    const validateForm = (): boolean => {
        try {
            schema.parse(toRaw(formData));
            // Clear all errors on successful validation
            Object.keys(errors).forEach(key => {
                (errors as any)[key] = '';
            });
            return true;
        } catch (err) {
            if (err instanceof z.ZodError) {
                err.issues.forEach((error: z.ZodIssue) => {
                    const fieldName = error.path[0] as string;
                    if (fieldName) {
                        (errors as any)[fieldName] = error.message;
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
            await onSubmit(toRaw(formData) as T);
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
            (errors as any)[key] = '';
        });
    };

    return {
        formData,
        errors,
        isSubmitting,
        clearFieldError,
        validateField,
        validateForm,
        handleSubmit,
        resetForm
    };
}
