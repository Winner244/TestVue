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

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
}

#app {
  min-height: 100vh;
  padding: 2rem 1rem;
}
</style>

<style scoped>
.app-container {
  max-width: 1200px;
  margin: 0 auto;
}

main {
  margin-top: 2rem;
}
</style>
