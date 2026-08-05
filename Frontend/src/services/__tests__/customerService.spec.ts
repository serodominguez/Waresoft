import { describe, it, expect, vi, beforeEach } from 'vitest';
import axios from 'axios';
import { customerService } from '../customerService';
import type { CustomerStats } from '@/interfaces/customerInterface';

// ─── Mock de axios ────────────────────────────────────────────────────────────

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

beforeEach(() => {
    vi.clearAllMocks();
});

// ─── Tests ────────────────────────────────────────────────────────────────────

describe('CustomerService.getStats', () => {

    it('llama a axios.get con el endpoint correcto', async () => {
        const stats: CustomerStats = {
            totalActive: 10,
            percentageChange: 15.5,
            isPositive: true,
        };
        mockedAxios.get.mockResolvedValueOnce({ data: { data: stats } });

        await customerService.getStats();

        expect(mockedAxios.get).toHaveBeenCalledWith('api/Customer/Stats');
    });

    it('retorna el objeto CustomerStats de la respuesta', async () => {
        const stats: CustomerStats = {
            totalActive: 10,
            percentageChange: 15.5,
            isPositive: true,
        };
        mockedAxios.get.mockResolvedValueOnce({ data: { data: stats } });

        const result = await customerService.getStats();

        expect(result).toEqual(stats);
    });

    it('propaga el error si axios falla', async () => {
        mockedAxios.get.mockRejectedValueOnce(new Error('Network Error'));

        await expect(customerService.getStats()).rejects.toThrow('Network Error');
    });
});