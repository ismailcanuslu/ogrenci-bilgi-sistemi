import React, { useState } from 'react';
import { Layout } from 'antd';
import { HeaderBar } from './HeaderBar';
import { AppSidebar } from './AppSidebar';
import { Outlet } from 'react-router-dom';

const { Content, Footer } = Layout;

export const MainLayout: React.FC = () => {
  const [collapsed, setCollapsed] = useState<boolean>(false);

  return (
    <Layout style={{ minHeight: '100vh', backgroundColor: '#f4f7fb' }}>
      <AppSidebar collapsed={collapsed} onCollapse={setCollapsed} />
      <Layout>
        <HeaderBar collapsed={collapsed} onToggleCollapse={() => setCollapsed(!collapsed)} />
        <Content
          style={{
            margin: '24px 24px 0',
            overflow: 'initial',
            minHeight: 'calc(100vh - 150px)',
          }}
        >
          <Outlet />
        </Content>
        <Footer
          style={{
            textAlign: 'center',
            color: '#64748b',
            fontSize: 12,
            padding: '20px 24px',
            backgroundColor: 'transparent',
          }}
        >
          T.C. Üniversite Bilgi Yönetim Sistemi © 2026 | Tüm Hakları Saklıdır | Destek & İletişim: bim@obs.edu.tr
        </Footer>
      </Layout>
    </Layout>
  );
};
