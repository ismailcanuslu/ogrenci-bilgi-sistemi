import React, { useEffect, useState } from 'react';
import { Card, Descriptions, Tag, Typography, Row, Col, Alert, Input, Button, Space, message } from 'antd';
import {
  BankOutlined,
  SafetyCertificateOutlined,
  DollarOutlined,
  CheckCircleOutlined,
} from '@ant-design/icons';
import { studentPortalApi } from '../../api/studentPortal';
import type { TuitionStatus, MilitaryStatus } from '../../api/studentPortal';

const { Title, Text } = Typography;

export const StudentDetailsPage: React.FC = () => {
  const [tuition, setTuition] = useState<TuitionStatus | null>(null);
  const [military, setMilitary] = useState<MilitaryStatus | null>(null);
  const [iban, setIban] = useState<string>('TR44 0006 2000 0001 2345 6789 01');

  useEffect(() => {
    studentPortalApi.getTuition().then(setTuition);
    studentPortalApi.getMilitary().then(setMilitary);
  }, []);

  const handleUpdateIban = () => {
    message.success('Banka IBAN bilgisi başarıyla güncellendi.');
  };

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 20,
          padding: '20px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Öğrenci Özllik & Mali/Hukuki Durum Kartı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Harç ve Mali İşlemler, Askerlik Durumu, IBAN ve Disiplin Sicil Bilgileri
          </Text>
        </div>
      </div>

      <Row gutter={[20, 20]}>
        {/* Harç ve Mali Durum */}
        <Col xs={24} md={12}>
          <Card
            title={
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <DollarOutlined style={{ color: '#10b981' }} />
                <span>Harç & Katkı Payı Durumu</span>
              </div>
            }
            extra={<Tag color="green">Borç Bulunmamaktadır</Tag>}
            style={{ borderRadius: 10, height: '100%' }}
          >
            <Descriptions column={1} bordered size="small">
              <Descriptions.Item label="Aktif Dönem">{tuition?.termName || '2026-2027 Güz'}</Descriptions.Item>
              <Descriptions.Item label="Tahakkuk Eden">{tuition?.amountDue || 0} ₺</Descriptions.Item>
              <Descriptions.Item label="Ödenen Tutar">{tuition?.amountPaid || 0} ₺</Descriptions.Item>
              <Descriptions.Item label="Kayıt Durumu">
                <Tag color="success">Ders Kaydı Yapabilir (Mali Engel Yok)</Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Banka Hesap">{tuition?.bankAccount}</Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        {/* Askerlik Durumu */}
        <Col xs={24} md={12}>
          <Card
            title={
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <SafetyCertificateOutlined style={{ color: '#0f4c81' }} />
                <span>Askerlik Durumu (ASAL Bilgileri)</span>
              </div>
            }
            extra={<Tag color="blue">Tecilli</Tag>}
            style={{ borderRadius: 10, height: '100%' }}
          >
            <Descriptions column={1} bordered size="small">
              <Descriptions.Item label="ASAL Durumu">{military?.status || 'Öğrenim Nedeniyle Tecilli'}</Descriptions.Item>
              <Descriptions.Item label="Tecil Bitiş Tarihi">{military?.postponementDate || '31.12.2028'}</Descriptions.Item>
              <Descriptions.Item label="Belge Barkod">{military?.asalDocumentNumber || 'ASAL-2021-987452'}</Descriptions.Item>
              <Descriptions.Item label="YÖK Bildirimi">
                <Tag color="green" icon={<CheckCircleOutlined />}>Otomatik Entegre Edildi</Tag>
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        {/* Banka IBAN Bilgileri */}
        <Col xs={24} md={12}>
          <Card
            title={
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <BankOutlined style={{ color: '#6366f1' }} />
                <span>Burs / Harç İadesi IBAN Bilgileri</span>
              </div>
            }
            style={{ borderRadius: 10 }}
          >
            <Text type="secondary" style={{ fontSize: 13, display: 'block', marginBottom: 12 }}>
              Üniversitemiz tarafından yapılacak burs, staj ücreti ve katkı payı iadeleri için adınıza kayıtlı IBAN numaranızı giriniz.
            </Text>
            <Space.Compact style={{ width: '100%' }}>
              <Input value={iban} onChange={(e) => setIban(e.target.value)} />
              <Button type="primary" onClick={handleUpdateIban} style={{ backgroundColor: '#0f4c81' }}>
                Güncelle
              </Button>
            </Space.Compact>
          </Card>
        </Col>

        {/* Disiplin & Sicil Durumu */}
        <Col xs={24} md={12}>
          <Card
            title="Disiplin & Sicil Kaydı"
            extra={<Tag color="green">Temiz</Tag>}
            style={{ borderRadius: 10 }}
          >
            <Alert
              message="Herhangi bir disiplin veya ceza kaydınız bulunmamaktadır."
              type="success"
              showIcon
              style={{ borderRadius: 8 }}
            />
          </Card>
        </Col>
      </Row>
    </div>
  );
};
