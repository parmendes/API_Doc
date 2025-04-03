import React from 'react';
import SwaggerUI from 'swagger-ui-react';
import 'swagger-ui-react/swagger-ui.css';

interface SwaggerUIProps {
  specUrl: string;
}

const SwaggerUIComponent: React.FC<SwaggerUIProps> = ({ specUrl }) => {
  return <SwaggerUI url={specUrl} />;
};

export default SwaggerUIComponent;