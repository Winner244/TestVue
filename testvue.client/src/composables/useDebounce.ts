import { ref, watch } from 'vue';

/**
 * Composable for debouncing a value
 * @param value - The reactive value to debounce
 * @param delay - Delay in milliseconds (default: 300ms)
 * @returns Debounced value
 */
export function useDebounce<T>(value: T, delay: number = 300) {
    const debouncedValue = ref<T>(value);
    let timeout: ReturnType<typeof setTimeout> | null = null;

    watch(
        () => value,
        (newValue) => {
            if (timeout) {
                clearTimeout(timeout);
            }

            timeout = setTimeout(() => {
                debouncedValue.value = newValue as T;
            }, delay);
        }
    );

    return debouncedValue;
}
