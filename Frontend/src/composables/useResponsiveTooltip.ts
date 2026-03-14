import { computed } from 'vue';
import { useDisplay } from 'vuetify';

export const useResponsiveTooltip = () => {
  const { mobile, smAndDown } = useDisplay();
  
  // Puedes ajustar según tus necesidades
  const shouldDisableTooltip = computed(() => mobile.value || smAndDown.value);
  
  return {
    // Spread directamente en v-tooltip
    tooltipProps: computed(() => ({
      disabled: shouldDisableTooltip.value,
      openDelay: 500,
      closeDelay: 100
    })),
    
    // O usar individualmente
    disableTooltip: shouldDisableTooltip,
    isMobile: mobile,
    isSmallScreen: smAndDown
  };
};