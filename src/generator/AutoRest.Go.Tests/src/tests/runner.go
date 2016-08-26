package main

import (
	"bytes"
	"fmt"
	"os"
	"os/exec"
	"tests/acceptancetests/utils"
	"tests/generated/report"
)

func main() {
	server, err := startServer()
	if err != nil {
		fmt.Printf("Error! %v\n", err)
	}
	allPass := true
	runTests(&allPass)
	getReport()
	server.Kill()
	if !allPass {
		fmt.Println("Not all tests passed")
	}
}

func startServer() (*os.Process, error) {
	fmt.Println("Go Tests.......")
	testServerPath := "../../../../dev/TestServer/server"
	install := exec.Command("npm", "install")
	install.Dir = testServerPath
	server := exec.Command("node", "startup/www")
	server.Dir = testServerPath

	if err := install.Run(); err != nil {
		return install.Process, err
	}
	if err := server.Start(); err != nil {
		return server.Process, err
	}
	return server.Process, nil
}

func runTests(allPass *bool) {
	fmt.Println("Run tests")
	testSuites := []string{
		"arraygroup",
		"booleangroup",
		"bytegroup",
		"complexgroup",
		"dategroup",
		"datetimegroup",
		"datetimerfc1123group",
		"dictionarygroup",
		"durationgroup",
		"headergroup",
		"httpInfrastructuregroup",
		"integergroup",
		"modelflatteninggroup",
		"numbergroup",
		"requiredoptionalgroup",
		"stringgroup",
		"urlgroup",
		"validationgroup",
		"custombaseurlgroup",
		"filegroup",
		// "formdatagroup",
	}

	for _, suite := range testSuites {
		fmt.Printf("Run test (go test ./acceptancetests/%vtest -v) ...\n", suite)
		tests := exec.Command("go", "test", fmt.Sprintf("./acceptancetests/%vtest", suite), "-v")
		var stdout, stderr bytes.Buffer
		tests.Stdout, tests.Stderr = &stdout, &stderr
		err := tests.Run()
		fmt.Println(stdout.String())
		fmt.Println(stderr.String())
		if err != nil {
			fmt.Printf("Error! %v\n", err)
			*allPass = false
		}
		if stderr.String()[:2] != "OK" {
			*allPass = false
		}
	}
}

func getReport() {
	var reportClient = report.NewWithBaseURI(utils.GetBaseURI())
	res, err := reportClient.GetReport()
	if err != nil {
		fmt.Println("Error:", err)
	}

	count := 0
	for key, val := range *res.Value {
		if *val <= 0 {
			fmt.Println(key, *val)
			count++
		}
	}
	total := len(*res.Value)
	fmt.Printf("\nReport:	Passed(%v)  Failed(%v)\n", total-count, count)
	fmt.Println("Go Done.......\n")
}
