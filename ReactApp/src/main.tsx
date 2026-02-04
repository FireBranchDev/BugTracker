import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import HomePage from './components/pages/HomePage';
import './index.css';
import { DetailedBugPage, ProjectSettings } from './components/pages';
import CreateUserAccountPage from './components/pages/CreateUserAccountPage';
import ErrorPage from './components/pages/ErrorPage';
import ForbiddenPage from './components/pages/ForbiddenPage';
import NotFoundPage from './components/pages/NotFoundPage';
import ProjectPage from './components/pages/ProjectPage';
import ProjectsPage from './components/pages/ProjectsPage';
import AuthProtectedRoute from './components/routes/AuthProtectedRoute';
import UserRequiredProtectedRoute from './components/routes/UserAccountRequiredProtectedRoute';
import { CREATE_USER_ACCOUNT_PAGE_PATH } from './constants';
import SignupPage from './pages/SignupPage';

const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
    errorElement: <ErrorPage />,
    children: [
      {
        index: true,
        element: <HomePage />,
      },
      {
        element: <AuthProtectedRoute />,
        children: [
          {
            element: <UserRequiredProtectedRoute />,
            children: [
              {
                path: 'projects',
                element: <ProjectsPage />,
              },
              {
                path: 'projects/:projectId',
                element: <ProjectPage />,
              },
              {
                path: 'projects/:projectId/settings',
                element: <ProjectSettings />,
              },
              {
                path: 'projects/:projectId/bugs/:bugId',
                element: <DetailedBugPage />,
              },
            ],
          },
          {
            path: CREATE_USER_ACCOUNT_PAGE_PATH,
            element: <CreateUserAccountPage />,
          },
          {
            path: '/signup',
            element: <SignupPage />,
          },
        ],
      },
      {
        path: '/forbidden',
        element: <ForbiddenPage />,
      },
      {
        path: '/not-found',
        element: <NotFoundPage />,
      },

      {
        path: '*',
        element: <NotFoundPage />,
      },
    ],
  },
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
);
