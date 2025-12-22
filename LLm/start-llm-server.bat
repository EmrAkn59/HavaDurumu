@echo off
echo ========================================
echo Python LLM Sunucusu Baslatiliyor...
echo ========================================
echo.
echo Port: 5000
echo Klasor: %CD%
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
echo Gerekli paketler kontrol ediliyor...
python -c "import flask" 2>nul
if errorlevel 1 (
    echo Flask yukleniyor...
    pip install flask
)

python -c "import flask_cors" 2>nul
if errorlevel 1 (
    echo Flask-CORS yukleniyor...
    pip install flask-cors
)

python -c "import joblib" 2>nul
if errorlevel 1 (
    echo Joblib yukleniyor...
    pip install joblib
)

python -c "import pandas" 2>nul
if errorlevel 1 (
    echo Pandas yukleniyor...
    pip install pandas
)

python -c "import numpy" 2>nul
if errorlevel 1 (
    echo NumPy yukleniyor...
    pip install numpy
)

python -c "import sklearn" 2>nul
if errorlevel 1 (
    echo Scikit-learn yukleniyor...
    pip install scikit-learn
)

echo.
echo ========================================
echo Sunucu baslatiliyor...
echo ========================================
echo.
echo Sunucuyu durdurmak icin Ctrl+C basin
echo.

python app.py

pause

