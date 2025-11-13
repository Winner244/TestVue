<script setup lang="ts">
defineProps<{
  activeTab: 'contact' | 'order' | 'submissions';
}>();

const emit = defineEmits<{
  (e: 'update:activeTab', value: 'contact' | 'order' | 'submissions'): void;
}>();

const handleTabClick = (tab: 'contact' | 'order' | 'submissions') => {
  emit('update:activeTab', tab);
};
</script>

<template>
  <header class="header">
    <h1 class="header__title">Form Submission System</h1>
    <p class="header__subtitle">Demonstrating dynamic form handling with Vue.js & ASP.NET Core</p>
    <nav class="header__nav">
      <button
        @click="handleTabClick('contact')"
        :class="['header__tab', { 'header__tab--active': activeTab === 'contact' }]"
      >
        Contact Form
      </button>
      <button
        @click="handleTabClick('order')"
        :class="['header__tab', { 'header__tab--active': activeTab === 'order' }]"
      >
        Order Form
      </button>
      <button
        @click="handleTabClick('submissions')"
        :class="['header__tab', { 'header__tab--active': activeTab === 'submissions' }]"
      >
        View Submissions
      </button>
    </nav>
  </header>
</template>
