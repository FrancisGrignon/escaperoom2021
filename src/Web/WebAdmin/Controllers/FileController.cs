using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebAdmin.Clients;
using WebAdmin.Models;

namespace WebAdmin.Controllers
{
    public class FileController : Controller
    {
        private readonly IBackendClient _backendClient;

        public FileController(IBackendClient backendClient)
        {
            _backendClient = backendClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            if (null == files || 0 == files.Count)
            {
                return NotFound();
            }

            var file = files[0];
            var extension = Path.GetExtension(file.FileName);

            if (".xlsx" != extension)
            {
                return NotFound();
            }
 
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var contacts = ReadExcelSheet(stream, true);

                foreach (var contact in contacts)
                {
                    Console.WriteLine($"{contact.Name}, {contact.Email}");

                    await _backendClient.CreateAsync(contact);
                }
            }

            ViewBag.Message = "File successfully uploaded.";

            return View(); // RedirectToAction("Index");
        }

        private IEnumerable<Contact> ReadExcelSheet(Stream stream, bool firstRowIsHeader)
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
            {
                // Read the first Sheets 
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                int counter = 0, index = 0;
                Contact dto;

                foreach (Row row in rows)
                {
                    counter = counter + 1;

                    // Read the first row as header
                    if (counter == 1)
                    {
                        // Ignore header
                    }
                    else
                    {
                        dto = new Contact();

                        index = 0;

                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            if (0 == index)
                            {
                                dto.Name = GetCellValue(doc, cell);
                            }
                            else if (1 == index)
                            {
                                dto.Email = GetCellValue(doc, cell);
                            }
                            else
                            {
                                throw new NotSupportedException("Column #{i} not supported.");
                            }

                            index++;
                        }

                        yield return dto;
                    }
                }
            }
        }

        private string GetCellValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }

            return value;
        }
    }
}