import { FC, useContext } from 'react';
import DefaultPageLayout from '../components/default-page-layout';

import { AppContext } from '../contexts';

const ProfilePage: FC = () => {
  const appContext = useContext(AppContext);
  console.log(appContext);
  return (
    <>
      <DefaultPageLayout title="Profile">
        {appContext.displayName}
      </DefaultPageLayout>
    </>
  );
};

export default ProfilePage;
