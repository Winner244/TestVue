<script setup lang="ts">
    import { ref } from 'vue';
    import Header from './components/Header.vue';
    import ContactForm from './components/ContactForm.vue';
    import OrderForm from './components/OrderForm.vue';
    import SubmissionsList from './components/SubmissionsList.vue';

    const activeTab = ref<'contact' | 'order' | 'submissions'>('contact');
</script>

<template>
  <div class="app-container">
    <Header :activeTab="activeTab" @update:activeTab="activeTab = $event" />

    <main>
      <ContactForm v-if="activeTab === 'contact'" />
      <OrderForm v-else-if="activeTab === 'order'" />
      <SubmissionsList v-else />
    </main>
  </div>
</template>
