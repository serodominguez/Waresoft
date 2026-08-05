import { describe, it, expect, vi, beforeEach } from 'vitest';
import { ref } from 'vue';

// Mock de Vuetify antes del import del composable
vi.mock('vuetify', () => ({
  useDisplay: vi.fn()
}));

import { useDisplay } from 'vuetify';
import { useResponsiveTooltip } from '../useResponsiveTooltip';

const mockUseDisplay = vi.mocked(useDisplay);

describe('useResponsiveTooltip', () => {

  beforeEach(() => {
    mockUseDisplay.mockReturnValue({
      mobile: ref(false),
      smAndDown: ref(false),
    } as any);
  });

  it('tooltipProps tiene openDelay 500 y closeDelay 100', () => {
    const { tooltipProps } = useResponsiveTooltip();

    expect(tooltipProps.value.openDelay).toBe(500);
    expect(tooltipProps.value.closeDelay).toBe(100);
  });

  it('tooltip habilitado cuando no es móvil ni pantalla pequeña', () => {
    const { tooltipProps, disableTooltip } = useResponsiveTooltip();

    expect(tooltipProps.value.disabled).toBe(false);
    expect(disableTooltip.value).toBe(false);
  });

  it('tooltip deshabilitado cuando es móvil', () => {
    mockUseDisplay.mockReturnValue({
      mobile: ref(true),
      smAndDown: ref(false),
    } as any);

    const { tooltipProps, disableTooltip } = useResponsiveTooltip();

    expect(tooltipProps.value.disabled).toBe(true);
    expect(disableTooltip.value).toBe(true);
  });

  it('tooltip deshabilitado cuando es pantalla pequeña', () => {
    mockUseDisplay.mockReturnValue({
      mobile: ref(false),
      smAndDown: ref(true),
    } as any);

    const { tooltipProps, disableTooltip } = useResponsiveTooltip();

    expect(tooltipProps.value.disabled).toBe(true);
    expect(disableTooltip.value).toBe(true);
  });

  it('isMobile refleja el valor de mobile', () => {
    mockUseDisplay.mockReturnValue({
      mobile: ref(true),
      smAndDown: ref(false),
    } as any);

    const { isMobile } = useResponsiveTooltip();

    expect(isMobile.value).toBe(true);
  });

  it('isSmallScreen refleja el valor de smAndDown', () => {
    mockUseDisplay.mockReturnValue({
      mobile: ref(false),
      smAndDown: ref(true),
    } as any);

    const { isSmallScreen } = useResponsiveTooltip();

    expect(isSmallScreen.value).toBe(true);
  });
});