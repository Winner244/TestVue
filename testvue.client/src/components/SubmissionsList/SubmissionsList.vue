<template>
  <div class="submissions-container">
    <h2>Form Submissions</h2>
    
    <!-- Search Bar -->
    <div class="search-bar">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="Search submissions..."
        class="search-input"
      />
      <button @click="loadSubmissions" class="btn-refresh">Refresh</button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading">Loading submissions...</div>

    <!-- No Results -->
    <div v-else-if="filteredSubmissions.length === 0" class="no-results">
      No submissions found.
    </div>

    <!-- Submissions List -->
    <div v-else class="submissions-list">
      <div
        v-for="submission in filteredSubmissions"
        :key="submission.id"
        class="submission-card"
      >
        <div class="submission-header">
          <span class="submission-id">ID: {{ submission.id }}</span>
          <span class="submission-date">{{ formatDate(submission.submittedAt) }}</span>
        </div>
        <div class="submission-body">
          <div
            v-for="(value, key) in submission.formData"
            :key="key"
            class="field-row"
          >
            <span class="field-label">{{ formatFieldName(key) }}:</span>
            <span class="field-value">{{ formatFieldValue(value) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
    import { ref, computed, onMounted } from 'vue';

    interface Submission {
    id: string;
    formData: Record<string, any>;
    submittedAt: string;
    }

    const submissions = ref<Submission[]>([]);
    const searchQuery = ref('');
    const loading = ref(false);

    const filteredSubmissions = computed(() => {
    if (!searchQuery.value.trim()) {
        return submissions.value;
    }

    const query = searchQuery.value.toLowerCase();
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
        if (Array.isArray(value)) {
            return value.some(item => 
            typeof item === 'string' && item.toLowerCase().includes(query)
            );
        }
        return false;
        });
    });
    });

    const loadSubmissions = async () => {
    loading.value = true;
    try {
        const response = await fetch('/api/formsubmission');
        if (response.ok) {
        submissions.value = await response.json();
        } else {
        console.error('Failed to load submissions');
        }
    } catch (error) {
        console.error('Error loading submissions:', error);
    } finally {
        loading.value = false;
    }
    };

    const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString();
    };

    const formatFieldName = (key: string): string => {
    // Convert camelCase to Title Case
    return key
        .replace(/([A-Z])/g, ' $1')
        .replace(/^./, str => str.toUpperCase());
    };

    const formatFieldValue = (value: any): string => {
    if (value === null || value === undefined) {
        return 'N/A';
    }
    if (typeof value === 'boolean') {
        return value ? 'Yes' : 'No';
    }
    if (Array.isArray(value)) {
        return value.length > 0 ? value.join(', ') : 'None';
    }
    return String(value);
    };

    onMounted(() => {
    loadSubmissions();
    });
</script>

<style scoped lang="less" src="./submissions-list.less"></style>
