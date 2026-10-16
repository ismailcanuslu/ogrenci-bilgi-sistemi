import type { ThemeConfig } from 'antd';

export const corporateTheme: ThemeConfig = {
  token: {
    colorPrimary: '#0f4c81', // Kurumsal Klasik Lacivert / Oxford Mavi
    colorPrimaryHover: '#1b65a5',
    colorPrimaryActive: '#092c4f',
    colorSuccess: '#10b981',
    colorWarning: '#f59e0b',
    colorError: '#ef4444',
    colorInfo: '#0284c7',
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif",
    borderRadius: 8,
    borderRadiusLG: 10,
    borderRadiusSM: 6,
    colorBgBase: '#ffffff',
    colorBgLayout: '#f4f7fb',
    colorTextBase: '#1e293b',
    colorTextSecondary: '#64748b',
    colorBorder: '#e2e8f0',
    colorBorderSecondary: '#f1f5f9',
    fontSize: 14,
    fontSizeHeading1: 28,
    fontSizeHeading2: 22,
    fontSizeHeading3: 18,
    fontSizeHeading4: 15,
  },
  components: {
    Card: {
      headerBg: '#fafcff',
      headerFontSize: 15,
      paddingLG: 20,
    },
    Button: {
      controlHeight: 38,
      fontWeight: 500,
      paddingInline: 18,
    },
    Menu: {
      itemBorderRadius: 8,
      itemMarginInline: 8,
      itemSelectedColor: '#0f4c81',
      itemSelectedBg: '#e8f2fc',
    },
    Tabs: {
      titleFontSize: 14,
    },
    Table: {
      headerBg: '#f8fafc',
      headerColor: '#475569',
      headerSplitColor: 'transparent',
      rowHoverBg: '#f1f5f9',
    },
  },
};
