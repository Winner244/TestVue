import { ref, onMounted } from 'vue';
import { useToast } from 'vue-toast-notification';

interface Submission {
    id: string;
    formData: Record<string, any>;
    submittedAt: string;
}

/**
 * Composable for fetching and managing form submissions
 * @param apiEndpoint - API endpoint to fetch submissions from
 * @returns Submission management utilities
 */
export function useSubmissions(apiEndpoint: string = '/api/formsubmission') {
    const $toast = useToast();
    const submissions = ref<Submission[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);

    /**
     * Load submissions from the API
     */
    const loadSubmissions = async () => {
        loading.value = true;
        error.value = null;

        try {
            const response = await fetch(apiEndpoint);
            
            if (response.ok) {
                submissions.value = await response.json();
            } else {
                const errorMessage = 'Failed to load submissions. Please try again.';
                error.value = errorMessage;
                console.error('Error loading submissions:', response);
                $toast.error(errorMessage, {
                    position: 'top',
                    duration: 5000
                });
            }
        } catch (err) {
            const errorMessage = 'An error occurred while loading submissions. Please try again.';
            error.value = errorMessage;
            console.error('Error loading submissions:', err);
            $toast.error(errorMessage, {
                position: 'top',
                duration: 5000
            });
        } finally {
            loading.value = false;
        }
    };

    /**
     * Auto-load submissions on mount
     */
    onMounted(() => {
        loadSubmissions();
    });

    return {
        submissions,
        loading,
        error,
        loadSubmissions
    };
}
