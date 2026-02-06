import { BaseService } from './baseService';
import { Module } from '@/interfaces/moduleInterface';

class ModuleService extends BaseService<Module> {
  constructor() {
    super({
      endpoint: 'Module',
      downloadFileName: 'Módulos',
    });
  }
}

export const moduleService = new ModuleService();