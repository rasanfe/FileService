namespace FileService
{
    /// <summary>
    /// Utilidades de rutas y ficheros para consumir desde PowerBuilder, apoyadas en las
    /// clases managed de .NET (<see cref="System.IO.Path"/>, <see cref="System.IO.File"/>,
    /// <see cref="System.IO.Directory"/>).
    /// <para>
    /// La gracia es no reinventar en PB lo que .NET ya resuelve de serie y, además, de
    /// forma multiplataforma y robusta: separar nombre/extensión/carpeta de una ruta,
    /// renombrar, copiar árboles de directorios, listar ficheros, etc. Fijaos en que no
    /// hay P/Invoke aquí: todo es .NET puro, mucho más cómodo que pelearse con la API Win32.
    /// </para>
    /// </summary>
    public class FileService
    {
        // Guarda el texto del último error. Lo dejamos preparado para el patrón
        // "GetLastError" típico de estos ejemplos (PB consultaría el último error tras
        // un fallo). Los métodos que no pueden continuar lanzan además una excepción
        // para que el TRY...CATCH de PowerBuilder también se entere.
        private string errorText = "";

        /// <summary>
        /// Extrae el nombre del fichero (con extensión) de una ruta completa.
        /// </summary>
        /// <param name="fileInput">Ruta de entrada, p. ej. <c>C:\datos\informe.pdf</c>.</param>
        /// <returns>El nombre del fichero (<c>informe.pdf</c>), o cadena vacía si falla.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public string GetFilename(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }

            try
            {
                string fileOut = Path.GetFileName(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>
        /// Devuelve la extensión de un fichero, incluido el punto.
        /// </summary>
        /// <param name="fileInput">Ruta o nombre de fichero.</param>
        /// <returns>La extensión (p. ej. <c>.pdf</c>), o cadena vacía si falla o no tiene.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public string GetExtension(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            try
            {
                string fileOut = Path.GetExtension(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>
        /// Devuelve el nombre del fichero sin la extensión.
        /// </summary>
        /// <param name="fileInput">Ruta o nombre de fichero, p. ej. <c>informe.pdf</c>.</param>
        /// <returns>El nombre sin extensión (<c>informe</c>), o cadena vacía si falla.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public string GetFileNameWithoutExtension(string fileInput)
        {
            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            try
            {
                string fileOut = Path.GetFileNameWithoutExtension(fileInput);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>
        /// Cambia la extensión de una ruta de fichero (no toca el disco, solo la cadena).
        /// </summary>
        /// <param name="fileInput">Ruta o nombre original, p. ej. <c>informe.pdf</c>.</param>
        /// <param name="extension">Nueva extensión, con o sin punto (p. ej. <c>.txt</c>).</param>
        /// <returns>La ruta con la nueva extensión, o cadena vacía si falla.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public string ChangeExtension(string fileInput, string extension)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }

            try
            {
                string fileOut = Path.ChangeExtension(fileInput, extension);
                return fileOut;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>
        /// Devuelve la carpeta que contiene a la ruta indicada.
        /// </summary>
        /// <param name="fileInput">Ruta completa, p. ej. <c>C:\datos\informe.pdf</c>.</param>
        /// <returns>La carpeta (<c>C:\datos</c>), o cadena vacía si no hay o falla.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public string GetDirectoryName(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (EndsInDirectorySeparator(fileInput))
            {
                fileInput = Path.TrimEndingDirectorySeparator(fileInput);
            }


            try
            {
                // OJO: Path.GetDirectoryName puede devolver null (p. ej. en una raíz como "C:\").
                // Con Nullable activado lo marcamos con 'string?' y el operador ?? convierte
                // ese null en cadena vacía, que es más cómoda de tratar desde PowerBuilder.
                string? fileOut = Path.GetDirectoryName(fileInput);
                return fileOut ?? string.Empty;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return System.String.Empty;
            }
        }

        /// <summary>
        /// Indica si la ruta termina en separador de directorio (<c>\</c> o <c>/</c>).
        /// Lo usan internamente los demás métodos para limpiar la barra final antes de
        /// trocear la ruta.
        /// </summary>
        /// <param name="fileInput">Ruta a comprobar.</param>
        /// <returns><c>true</c> si acaba en separador; <c>false</c> en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="fileInput"/> es nulo o vacío.</exception>
        public bool EndsInDirectorySeparator(string fileInput)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            bool isEndsInDirectorySeparator = Path.EndsInDirectorySeparator(fileInput);

            return isEndsInDirectorySeparator;
        }

        /// <summary>
        /// Renombra (o mueve) un fichero. Si el destino ya existe, lo borra antes para
        /// poder sobrescribir.
        /// </summary>
        /// <param name="fileInput">Ruta del fichero origen (debe existir).</param>
        /// <param name="newFile">Ruta/nombre destino.</param>
        /// <returns><c>true</c> si se renombró; <c>false</c> si hubo un error controlado.</returns>
        /// <exception cref="ArgumentNullException">Si alguna ruta es nula o vacía.</exception>
        /// <exception cref="ArgumentException">Si el fichero origen no existe.</exception>
        public bool FileRename(string fileInput, string newFile)
        {

            if (string.IsNullOrEmpty(fileInput))
            {
                errorText = "Input File cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            if (!File.Exists(fileInput))
            {
                errorText = "Input File not Exists";
                throw new ArgumentException(paramName: nameof(fileInput), message: errorText);
            }

            if (string.IsNullOrEmpty(newFile))
            {
                errorText = "New File Name cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(fileInput), message: errorText);
            }

            try
            {
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                File.Move(fileInput, newFile);
                return true;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Copia un directorio completo (todos sus ficheros). Sobrecarga que copia todo
        /// el contenido usando el patrón <c>*</c>.
        /// </summary>
        /// <param name="sourceDir">Carpeta origen.</param>
        /// <param name="destinationDir">Carpeta destino (se crea si no existe).</param>
        /// <param name="recursive">Si <c>true</c>, copia también las subcarpetas.</param>
        /// <exception cref="DirectoryNotFoundException">Si el origen no existe.</exception>
        public void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            string source = @sourceDir;
            string destination = @destinationDir;
            try
            {
                DirectoryCopy(source, destination, recursive, "*");
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw new DirectoryNotFoundException(errorText);
            }

        }

        /// <summary>
        /// Copia un directorio filtrando los ficheros por un patrón de búsqueda.
        /// </summary>
        /// <param name="sourceDir">Carpeta origen.</param>
        /// <param name="destinationDir">Carpeta destino (se crea si no existe).</param>
        /// <param name="recursive">Si <c>true</c>, copia también las subcarpetas.</param>
        /// <param name="searchPattern">Filtro de ficheros, p. ej. <c>*.txt</c>.</param>
        /// <exception cref="DirectoryNotFoundException">Si el origen no existe.</exception>
        public void CopyDirectory(string sourceDir, string destinationDir, bool recursive, string searchPattern)
        {
            string source = @sourceDir;
            string destination = @destinationDir;
            try
            {
                DirectoryCopy(source, destination, recursive, searchPattern);
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw new DirectoryNotFoundException(errorText);
            }

        }
        // Motor de copia recursiva, privado: se llama a sí mismo por cada subcarpeta.
        // No es público porque es un detalle de implementación de CopyDirectory.
        static void DirectoryCopy(string sourceDir, string destinationDir, bool recursive, string searchPattern)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles(searchPattern))
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    DirectoryCopy(subDir.FullName, newDestinationDir, true, searchPattern);
                }
            }
        }

        /// <summary>
        /// Lista los ficheros de una carpeta devolviendo sus <b>rutas relativas</b> a la
        /// carpeta de origen. Cómodo para PowerBuilder, donde luego se recorre el array.
        /// </summary>
        /// <param name="sourceDir">Carpeta a explorar.</param>
        /// <param name="recursive">Si <c>true</c>, incluye también las subcarpetas.</param>
        /// <param name="searchPattern">Filtro de ficheros, p. ej. <c>*.pdf</c>.</param>
        /// <returns>Array de rutas relativas de los ficheros encontrados.</returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="sourceDir"/> es nulo o vacío.</exception>
        /// <exception cref="DirectoryNotFoundException">Si la carpeta no existe.</exception>
        public string[] GetDirectoryFiles(string sourceDir, bool recursive, string searchPattern)
        {
            if (string.IsNullOrEmpty(sourceDir))
            {
                errorText = "Source directory cannot be null or Empty";
                throw new ArgumentNullException(paramName: nameof(sourceDir), message: errorText);
            }

            try
            {
                var fileList = new List<string>();
                GetDirectoryFilesRecursive(sourceDir, sourceDir, recursive, searchPattern, fileList);
                return fileList.ToArray();
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw new DirectoryNotFoundException(errorText);
            }
        }

        // Recorrido recursivo auxiliar: acumula en 'fileList' las rutas relativas a 'baseDir'.
        // 'baseDir' se mantiene fijo en toda la recursión para calcular bien la ruta relativa.
        private static void GetDirectoryFilesRecursive(string baseDir, string currentDir, bool recursive, string searchPattern, List<string> fileList)
        {
            // Get information about the current directory
            var dir = new DirectoryInfo(currentDir);

            // Check if the current directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Directory not found: {dir.FullName}");

            // Get the files in the current directory and add to the list with relative paths
            foreach (FileInfo file in dir.GetFiles(searchPattern))
            {
                string relativePath = Path.GetRelativePath(baseDir, file.FullName);
                fileList.Add(relativePath);
            }

            // If recursive, process subdirectories
            if (recursive)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in dirs)
                {
                    GetDirectoryFilesRecursive(baseDir, subDir.FullName, recursive, searchPattern, fileList);
                }
            }
        }

    }
}