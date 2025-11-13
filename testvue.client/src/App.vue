<script setup lang="ts">
import { ref } from 'vue';
import Header from './components/Header/Header.vue';
import ContactForm from './components/ContactForm/ContactForm.vue';
import SubmissionsList from './components/SubmissionsList/SubmissionsList.vue';
import './assets/styles/app.less'

const activeTab = ref<'contact' | 'submissions'>('contact');
</script>

<template>
    <div class="app-container">
        <Header :activeTab="activeTab" @update:activeTab="activeTab = $event" />

        <main role="main" aria-label="Main content">
            <ContactForm v-if="activeTab === 'contact'" />
            <SubmissionsList v-else />
        </main>
    </div>
</template>
