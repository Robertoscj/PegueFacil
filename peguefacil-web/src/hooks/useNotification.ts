import { useSnackbar, VariantType } from 'notistack';

const useNotification = () => {
  const { enqueueSnackbar } = useSnackbar();

  const showNotification = (message: string, variant: VariantType = 'default') => {
    enqueueSnackbar(message, {
      variant,
      anchorOrigin: {
        vertical: 'top',
        horizontal: 'right',
      },
      autoHideDuration: 3000,
    });
  };

  const showSuccess = (message: string) => {
    showNotification(message, 'success');
  };

  const showError = (message: string) => {
    showNotification(message, 'error');
  };

  const showWarning = (message: string) => {
    showNotification(message, 'warning');
  };

  const showInfo = (message: string) => {
    showNotification(message, 'info');
  };

  return {
    showSuccess,
    showError,
    showWarning,
    showInfo,
  };
};

export default useNotification; 