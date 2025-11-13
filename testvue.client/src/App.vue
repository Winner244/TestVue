<script setup lang="ts">
    import { ref } from 'vue';
    import Header from './components/Header/Header.vue';
    import ContactForm from './components/ContactForm/ContactForm.vue';
    import OrderForm from './components/OrderForm/OrderForm.vue';
    import SubmissionsList from './components/SubmissionsList/SubmissionsList.vue';
    import './assets/styles/app.less'

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
