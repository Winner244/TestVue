import { createRouter, createWebHistory } from 'vue-router'
import ContactForm from '../components/ContactForm/ContactForm.vue'
import SubmissionsList from '../components/SubmissionsList/SubmissionsList.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'contact',
            component: ContactForm,
            meta: {
                title: 'Contact Form'
            }
        },
        {
            path: '/submissions',
            name: 'submissions',
            component: SubmissionsList,
            meta: {
                title: 'Submissions List'
            }
        },
        {
            path: '/:pathMatch(.*)*',
            name: 'not-found',
            redirect: '/'
        }
    ]
})

// Global navigation guard to handle server routes
router.beforeEach((to, _from, next) => {
    // Check if the route should be handled by the server
    if (to.path.startsWith('/api/') || 
        to.path.startsWith('/health') || 
        to.path.startsWith('/swagger')) {
        // Force full page navigation to server
        window.location.href = to.fullPath
        return // Don't call next() - we're leaving the app
    }
    
    document.title = (to.meta.title as string) || 'TestVue App'
    next()
})

export default router
