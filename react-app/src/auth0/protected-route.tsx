import {
  WithAuthenticationRequiredOptions,
  withAuthenticationRequired,
} from '@auth0/auth0-react';
import { ComponentType } from 'react';

type P = object;

interface IProtectedRoute {
  component: ComponentType<P>;
  options?: WithAuthenticationRequiredOptions;
}

const ProtectedRoute = ({ component, options }: IProtectedRoute) => {
  const MyProtectedComponent = withAuthenticationRequired(component, options);
  return <MyProtectedComponent />;
};

export default ProtectedRoute;
