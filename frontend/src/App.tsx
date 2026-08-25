import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { AreasPage } from './pages/AreasPage'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<AreasPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
