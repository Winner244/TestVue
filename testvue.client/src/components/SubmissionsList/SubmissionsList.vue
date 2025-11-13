<template>
    <div class="submissions">
        <h2 class="submissions__heading">Form Submissions</h2>

        <!-- Search Bar -->
        <div class="submissions__search" role="search" aria-label="Search submissions">
            <label for="search-input" class="sr-only">Search submissions</label>
            <input 
                type="search" 
                id="search-input"
                v-model="searchQuery" 
                placeholder="Search submissions..." 
                class="submissions__input"
                aria-label="Search through form submissions" />
            <button 
                @click="loadSubmissions" 
                class="submissions__button"
                aria-label="Refresh submissions list">
                Refresh
            </button>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="submissions__loading" role="status" aria-live="polite" aria-busy="true">
            <Loading 
                :active="loading" 
                :is-full-page="false"
                loader="spinner"
                color="#42b983"
                :width="64"
                :height="64"
                background-color="transparent" />
            <p class="submissions__loading-text">Loading submissions...</p>
        </div>

        <!-- No Results -->
        <div v-else-if="filteredSubmissions.length === 0" class="submissions__status" role="status" aria-live="polite">
            No submissions found.
        </div>

        <!-- Submissions List -->
        <div v-else class="submissions__list" role="region" aria-label="List of form submissions">
            <article v-for="submission in filteredSubmissions" :key="submission.id" class="submissions__card">
                <div class="submissions__card-header">
                    <span class="submissions__card-id">ID: {{ submission.id }}</span>
                    <time class="submissions__card-date" :datetime="submission.submittedAt">{{ formatDate(submission.submittedAt) }}</time>
                </div>
                <dl class="submissions__card-body">
                    <div v-for="(value, key) in submission.formData" :key="key" class="submissions__field">
                        <dt class="submissions__field-label">{{ formatFieldName(key) }}:</dt>
                        <dd class="submissions__field-value">{{ formatFieldValue(value) }}</dd>
                    </div>
                </dl>
            </article>
        </div>
    </div>
</template>

<style scoped lang="less" src="./submissions-list.less"></style>

<script setup lang="ts">
    import { ref, computed } from 'vue';
    import { formatDate, formatFieldName, formatFieldValue } from '../../utils/formatters';
    import { formatBooleanValue } from '../../constants/booleanDisplay';
    import { useSubmissions } from '../../composables/useSubmissions';
    import Loading from 'vue-loading-overlay';
    import 'vue-loading-overlay/dist/css/index.css';

    interface Submission {
        id: string;
        formData: Record<string, any>;
        submittedAt: string;
    }

    const searchQuery = ref('');

    // Use the submissions composable
    const { submissions, loading, loadSubmissions } = useSubmissions();

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
</script>
