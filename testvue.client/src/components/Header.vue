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
    <h1 class="header__title">Test ASP.NET + Vue</h1>
    <nav class="header__nav">
      <button
        @click="handleTabClick('contact')"
        :class="['header__tab', { 'header__tab--active': activeTab === 'contact' }]"
      >
        Contat Form
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
        View Result
      </button>
    </nav>
  </header>
</template>
