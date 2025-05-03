import react from 'react';
import Features from './home-page/features/features';
import Heading from './home-page/heading/heading';
import * as system from '@mui/system';
import DefaultPageLayout from '../components/default-page-layout';

const HomePage: react.FC = () => {
  return (
    <DefaultPageLayout title="BugTracker">
      <Heading />
      <system.Container maxWidth={false}>
        <Features />
      </system.Container>
    </DefaultPageLayout>
  );
};

export default HomePage;
