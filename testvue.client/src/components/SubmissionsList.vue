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

<style scoped>
.submissions-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

h2 {
  color: #2c3e50;
  margin-bottom: 1.5rem;
  text-align: center;
}

.search-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  align-items: center;
}

.search-input {
  flex: 1;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 4px;
  font-size: 1rem;
}

.search-input:focus {
  outline: none;
  border-color: #42b983;
}

.btn-refresh {
  padding: 0.75rem 1.5rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
}

.btn-refresh:hover {
  background-color: #359268;
}

.loading,
.no-results {
  text-align: center;
  padding: 2rem;
  color: #666;
  font-size: 1.1rem;
}

.submissions-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.submission-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  transition: box-shadow 0.3s;
}

.submission-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.submission-header {
  display: flex;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  background-color: #f8f9fa;
  border-bottom: 1px solid #e0e0e0;
}

.submission-id {
  font-weight: 600;
  color: #2c3e50;
}

.submission-date {
  color: #666;
  font-size: 0.9rem;
}

.submission-body {
  padding: 1.5rem;
}

.field-row {
  display: flex;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.field-row:last-child {
  border-bottom: none;
}

.field-label {
  flex: 0 0 200px;
  font-weight: 600;
  color: #2c3e50;
}

.field-value {
  flex: 1;
  color: #555;
  word-break: break-word;
}

@media (max-width: 768px) {
  .field-row {
    flex-direction: column;
    gap: 0.25rem;
  }

  .field-label {
    flex: none;
  }
}
</style>
