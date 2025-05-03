import { FC, useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { useNavigate } from 'react-router';
import DefaultPageLayout from '../components/default-page-layout';

const DashboardPage: FC = () => {
  const { isAuthenticated } = useAuth0();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isAuthenticated) navigate('/home');
  });

  return <DefaultPageLayout title="Dashboard" />;
};

export default DashboardPage;
