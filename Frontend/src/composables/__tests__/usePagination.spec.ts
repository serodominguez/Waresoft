import { describe, it, expect, vi } from 'vitest';
import { usePagination } from '../usePagination';

describe('usePagination', () => {

  it('inicializa con página 1 y 10 items por página', () => {
    const { currentPage, itemsPerPage } = usePagination(vi.fn());

    expect(currentPage.value).toBe(1);
    expect(itemsPerPage.value).toBe(10);
  });

  it('updateItemsPerPage actualiza itemsPerPage y resetea a página 1', () => {
    const { currentPage, itemsPerPage, updateItemsPerPage } = usePagination(vi.fn());

    currentPage.value = 5;
    updateItemsPerPage(25);

    expect(itemsPerPage.value).toBe(25);
    expect(currentPage.value).toBe(1);
  });

  it('updateItemsPerPage llama fetchFn con pageNumber 1 y el nuevo pageSize', () => {
    const fetchFn = vi.fn();
    const { updateItemsPerPage } = usePagination(fetchFn);

    updateItemsPerPage(25);

    expect(fetchFn).toHaveBeenCalledWith({ pageNumber: 1, pageSize: 25 });
  });

  it('changePage actualiza currentPage', () => {
    const { currentPage, changePage } = usePagination(vi.fn());

    changePage(3);

    expect(currentPage.value).toBe(3);
  });

  it('changePage llama fetchFn con la página y pageSize actuales', () => {
    const fetchFn = vi.fn();
    const { changePage } = usePagination(fetchFn);

    changePage(4);

    expect(fetchFn).toHaveBeenCalledWith({ pageNumber: 4, pageSize: 10 });
  });

  it('changePage usa el itemsPerPage actualizado', () => {
    const fetchFn = vi.fn();
    const { updateItemsPerPage, changePage } = usePagination(fetchFn);

    updateItemsPerPage(50);
    changePage(2);

    expect(fetchFn).toHaveBeenLastCalledWith({ pageNumber: 2, pageSize: 50 });
  });
});