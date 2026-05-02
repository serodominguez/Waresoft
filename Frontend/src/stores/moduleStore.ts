import { createBaseStore } from '@/stores/baseStore';
import { Module } from '@/interfaces/moduleInterface';
import { moduleService } from '@/services/moduleService';

export const useModuleStore = createBaseStore<Module>('module', moduleService)