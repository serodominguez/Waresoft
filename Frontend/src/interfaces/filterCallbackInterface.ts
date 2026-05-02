export interface FilterSearchParams {
  search: string | null;
  selectedFilter: string;
  state: string;
  startDate: Date | null;
  endDate: Date | null;
}

export interface FilterDownloadParams {
  search: string | null;
  selectedFilter: string;
  stateFilter: string;
  startDate: Date | null;
  endDate: Date | null;
}