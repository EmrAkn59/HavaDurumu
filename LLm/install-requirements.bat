@echo off
echo ========================================
echo Python Paketleri Yukleniyor...
echo ========================================
echo.

cd /d "%~dp0"

echo Python kontrol ediliyor...
python --version
if errorlevel 1 (
    echo HATA: Python bulunamadi! Python yuklu oldugundan emin olun.
    pause
    exit /b 1
)

echo.
echo Gerekli paketler yukleniyor...
echo.

echo [1/6] Flask yukleniyor...
pip install flask

echo [2/6] Flask-CORS yukleniyor...
pip install flask-cors

echo [3/6] Joblib yukleniyor...
pip install joblib

echo [4/6] Pandas yukleniyor...
pip install pandas

echo [5/6] NumPy yukleniyor...
pip install numpy

echo [6/6] Scikit-learn yukleniyor...
pip install scikit-learn

echo.
echo ========================================
echo Tum paketler basariyla yuklendi!
echo ========================================
echo.
pause

