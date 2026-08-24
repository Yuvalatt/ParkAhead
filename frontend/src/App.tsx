import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { AreasPage } from './pages/AreasPage'
import { ForecastPage } from './pages/ForecastPage'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AreasPage />} />
        <Route path="/areas/:areaId" element={<ForecastPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
