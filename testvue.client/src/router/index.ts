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


// Update page title on route change
router.beforeEach((to, _from, next) => {
  document.title = (to.meta.title as string) || 'TestVue App'
  next()
})

export default router
