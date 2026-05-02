import { createBaseStore } from '@/stores/baseStore';
import { Category } from '@/interfaces/categoryInterface';
import { categoryService } from '@/services/categoryService';

export const useCategoryStore = createBaseStore<Category>('category', categoryService);