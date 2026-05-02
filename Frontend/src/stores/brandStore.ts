import { createBaseStore } from '@/stores/baseStore';
import { Brand } from '@/interfaces/brandInterface';
import { brandService } from '@/services/brandService';

export const useBrandStore = createBaseStore<Brand>('brand', brandService)