<template>
    <div class="submissions">
        <h2 class="submissions__heading">Form Submissions</h2>

        <!-- Search Bar -->
        <div class="submissions__search">
            <input type="text" v-model="searchQuery" placeholder="Search submissions..." class="submissions__input" />
            <button @click="loadSubmissions" class="submissions__button">Refresh</button>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="submissions__status">Loading submissions...</div>

        <!-- No Results -->
        <div v-else-if="filteredSubmissions.length === 0" class="submissions__status">
            No submissions found.
        </div>

        <!-- Submissions List -->
        <div v-else class="submissions__list">
            <div v-for="submission in filteredSubmissions" :key="submission.id" class="submissions__card">
                <div class="submissions__card-header">
                    <span class="submissions__card-id">ID: {{ submission.id }}</span>
                    <span class="submissions__card-date">{{ formatDate(submission.submittedAt) }}</span>
                </div>
                <div class="submissions__card-body">
                    <div v-for="(value, key) in submission.formData" :key="key" class="submissions__field">
                        <span class="submissions__field-label">{{ formatFieldName(key) }}:</span>
                        <span class="submissions__field-value">{{ formatFieldValue(value) }}</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped lang="less" src="./submissions-list.less"></style>

<script setup lang="ts">
    import { ref, computed, onMounted } from 'vue';
    import { useToast } from 'vue-toast-notification';
    import { formatDate, formatFieldName, formatFieldValue } from '../../utils/formatters';
    import { formatBooleanValue } from '../../constants/booleanDisplay';

    interface Submission {
        id: string;
        formData: Record<string, any>;
        submittedAt: string;
    }

    const submissions = ref<Submission[]>([]);
    const searchQuery = ref('');
    const loading = ref(false);
    const $toast = useToast();

    const loadSubmissions = async () => {
        loading.value = true;
        try {
            const response = await fetch('/api/formsubmission');
            if (response.ok) {
                submissions.value = await response.json();
            } else {
                console.error('Error to load submissions:', response);
                $toast.error('Failed to load submissions. Please try again.', {
                    position: 'top',
                    duration: 5000
                });
            }
        } catch (error) {
            console.error('Error loading submissions:', error);
            $toast.error('An error occurred while loading submissions. Please try again.', {
                position: 'top',
                duration: 5000
            });
        } finally {
            loading.value = false;
        }
    };

    const filteredSubmissions = computed(() => {
        if (!searchQuery.value.trim()) {
            return submissions.value;
        }

        const query = searchQuery.value.trim().toLowerCase();
        return submissions.value.filter(submission => {
            // Search in ID
            if (submission.id.toLowerCase().includes(query)) {
                return true;
            }

            // Search in form data values
            return Object.values(submission.formData).some(value => {
                if (typeof value === 'string') {
                    return value.toLowerCase().includes(query);
                }
                
                if (typeof value === 'boolean') {
                    return formatBooleanValue(value).toLowerCase().includes(query);
                }
                
                if (Array.isArray(value)) {
                    return value.some(item =>
                        typeof item === 'string' && item.toLowerCase().includes(query)
                    );
                }
                return false;
            });
        });
    });

    onMounted(() => {
        loadSubmissions();
    });
</script>
