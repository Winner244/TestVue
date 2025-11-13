<template>
  <div class="form-container">
    <h2>Order Form</h2>
    <p class="form-description">
      This is a different form to demonstrate the backend works with any structure!
    </p>
    <form @submit.prevent="handleSubmit" class="order-form">
      <!-- Product Name -->
      <div class="form-group">
        <label for="productName">Product Name *</label>
        <input
          type="text"
          id="productName"
          v-model="formData.productName"
          @blur="validateField('productName')"
          :class="{ 'error': errors.productName }"
        />
        <span v-if="errors.productName" class="error-message">{{ errors.productName }}</span>
      </div>

      <!-- Quantity -->
      <div class="form-group">
        <label for="quantity">Quantity *</label>
        <input
          type="number"
          id="quantity"
          v-model.number="formData.quantity"
          min="1"
          @blur="validateField('quantity')"
          :class="{ 'error': errors.quantity }"
        />
        <span v-if="errors.quantity" class="error-message">{{ errors.quantity }}</span>
      </div>

      <!-- Product Category -->
      <div class="form-group">
        <label for="category">Product Category *</label>
        <select
          id="category"
          v-model="formData.category"
          @change="validateField('category')"
          :class="{ 'error': errors.category }"
        >
          <option value="">Select category</option>
          <option value="electronics">Electronics</option>
          <option value="clothing">Clothing</option>
          <option value="books">Books</option>
          <option value="food">Food & Beverages</option>
        </select>
        <span v-if="errors.category" class="error-message">{{ errors.category }}</span>
      </div>

      <!-- Delivery Date -->
      <div class="form-group">
        <label for="deliveryDate">Preferred Delivery Date *</label>
        <input
          type="date"
          id="deliveryDate"
          v-model="formData.deliveryDate"
          @blur="validateField('deliveryDate')"
          :class="{ 'error': errors.deliveryDate }"
        />
        <span v-if="errors.deliveryDate" class="error-message">{{ errors.deliveryDate }}</span>
      </div>

      <!-- Payment Method -->
      <div class="form-group">
        <label>Payment Method *</label>
        <div class="radio-group">
          <label class="radio-label">
            <input
              type="radio"
              name="paymentMethod"
              value="credit-card"
              v-model="formData.paymentMethod"
              @change="validateField('paymentMethod')"
            />
            Credit Card
          </label>
          <label class="radio-label">
            <input
              type="radio"
              name="paymentMethod"
              value="paypal"
              v-model="formData.paymentMethod"
              @change="validateField('paymentMethod')"
            />
            PayPal
          </label>
          <label class="radio-label">
            <input
              type="radio"
              name="paymentMethod"
              value="bank-transfer"
              v-model="formData.paymentMethod"
              @change="validateField('paymentMethod')"
            />
            Bank Transfer
          </label>
        </div>
        <span v-if="errors.paymentMethod" class="error-message">{{ errors.paymentMethod }}</span>
      </div>

      <!-- Additional Services -->
      <div class="form-group">
        <label>Additional Services</label>
        <div class="checkbox-group">
          <label class="checkbox-label">
            <input
              type="checkbox"
              value="gift-wrap"
              v-model="formData.additionalServices"
            />
            Gift Wrapping
          </label>
          <label class="checkbox-label">
            <input
              type="checkbox"
              value="express-delivery"
              v-model="formData.additionalServices"
            />
            Express Delivery
          </label>
          <label class="checkbox-label">
            <input
              type="checkbox"
              value="insurance"
              v-model="formData.additionalServices"
            />
            Shipping Insurance
          </label>
        </div>
      </div>

      <!-- Special Instructions -->
      <div class="form-group">
        <label for="specialInstructions">Special Instructions</label>
        <textarea
          id="specialInstructions"
          v-model="formData.specialInstructions"
          rows="4"
        ></textarea>
      </div>

      <!-- Terms Agreement -->
      <div class="form-group">
        <label class="checkbox-label">
          <input
            type="checkbox"
            v-model="formData.agreeToTerms"
          />
          I agree to the terms and conditions *
        </label>
        <span v-if="errors.agreeToTerms" class="error-message">{{ errors.agreeToTerms }}</span>
      </div>

      <!-- Submit Button -->
      <div class="form-actions">
        <button type="submit" class="btn-submit" :disabled="isSubmitting">
          {{ isSubmitting ? 'Placing Order...' : 'Place Order' }}
        </button>
      </div>

      <!-- Success Message -->
      <div v-if="successMessage" class="success-message">
        {{ successMessage }}
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
    import { reactive, ref } from 'vue';

    const formData = reactive({
    productName: '',
    quantity: 1,
    category: '',
    deliveryDate: '',
    paymentMethod: '',
    additionalServices: [] as string[],
    specialInstructions: '',
    agreeToTerms: false,
    formType: 'order' // Different form type identifier
    });

    const errors = reactive({
    productName: '',
    quantity: '',
    category: '',
    deliveryDate: '',
    paymentMethod: '',
    agreeToTerms: ''
    });

    const isSubmitting = ref(false);
    const successMessage = ref('');

    const validateField = (field: string) => {
    switch (field) {
        case 'productName':
        if (!formData.productName.trim()) {
            errors.productName = 'Product name is required';
        } else {
            errors.productName = '';
        }
        break;

        case 'quantity':
        if (!formData.quantity || formData.quantity < 1) {
            errors.quantity = 'Quantity must be at least 1';
        } else {
            errors.quantity = '';
        }
        break;

        case 'category':
        if (!formData.category) {
            errors.category = 'Please select a category';
        } else {
            errors.category = '';
        }
        break;

        case 'deliveryDate':
        if (!formData.deliveryDate) {
            errors.deliveryDate = 'Please select a delivery date';
        } else {
            const selectedDate = new Date(formData.deliveryDate);
            const tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            tomorrow.setHours(0, 0, 0, 0);
            if (selectedDate < tomorrow) {
            errors.deliveryDate = 'Delivery date must be at least tomorrow';
            } else {
            errors.deliveryDate = '';
            }
        }
        break;

        case 'paymentMethod':
        if (!formData.paymentMethod) {
            errors.paymentMethod = 'Please select a payment method';
        } else {
            errors.paymentMethod = '';
        }
        break;
    }
    };

    const validateForm = (): boolean => {
    validateField('productName');
    validateField('quantity');
    validateField('category');
    validateField('deliveryDate');
    validateField('paymentMethod');

    if (!formData.agreeToTerms) {
        errors.agreeToTerms = 'You must agree to the terms and conditions';
    } else {
        errors.agreeToTerms = '';
    }

    return !Object.values(errors).some(error => error !== '');
    };

    const handleSubmit = async () => {
    if (!validateForm()) {
        return;
    }

    isSubmitting.value = true;
    successMessage.value = '';

    try {
        const response = await fetch('/api/formsubmission', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
        });

        if (response.ok) {
        successMessage.value = 'Order placed successfully!';
        // Reset form
        Object.assign(formData, {
            productName: '',
            quantity: 1,
            category: '',
            deliveryDate: '',
            paymentMethod: '',
            additionalServices: [],
            specialInstructions: '',
            agreeToTerms: false,
            formType: 'order'
        });
        } else {
        alert('Failed to place order. Please try again.');
        }
    } catch (error) {
        console.error('Error placing order:', error);
        alert('An error occurred. Please try again.');
    } finally {
        isSubmitting.value = false;
    }
    };
</script>

<style scoped lang="less" src="./order-form.less"></style>
